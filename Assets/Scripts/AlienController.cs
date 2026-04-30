using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class AlienController : MonoBehaviour
{
    [Header("Movement Speeds (m/s — per 02_player.yaml)")]
    [SerializeField] private float walkSpeed   = 1.39f;
    [SerializeField] private float runSpeed    = 3.06f;
    [SerializeField] private float sprintSpeed = 8.33f;
    [SerializeField] private float backSpeed   = 1.39f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.4f;
    [SerializeField] private float gravity    = -20f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool  debugLog      = false;
    [SerializeField] private float debugInterval = 0.2f;

    // ── Inventory / Weapon Selection ─────────────────────────────────────────
    public static readonly string[] WeaponNames = { "Pistol", "Crossbow", "Fists" };
    private int _weaponIndex = 0;
    public int WeaponIndex => _weaponIndex;

    // ── Internal state ────────────────────────────────────────────────────────
    private CharacterController _cc;
    private Animator            _anim;
    private Vector3             _velocity;
    private bool                _isGrounded;
    private float               _debugTimer;
    private Vector3             _lastPos;
    private Transform           _probeBoneHips;
    private Transform           _probeBoneLFoot;
    private bool                _jumpInitiated;

    private static readonly int P_VelX      = Animator.StringToHash("VelocityX");
    private static readonly int P_VelZ      = Animator.StringToHash("VelocityZ");
    private static readonly int P_Grounded  = Animator.StringToHash("IsGrounded");
    private static readonly int P_Jump      = Animator.StringToHash("Jump");
    private static readonly int P_IsFalling = Animator.StringToHash("IsFalling");

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _cc   = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            if (_probeBoneHips  == null && t.name == "mixamorig:Hips")      _probeBoneHips  = t;
            if (_probeBoneLFoot == null && t.name == "mixamorig:LeftFoot")  _probeBoneLFoot = t;
        }
    }

    private void Start()
    {
        if (_anim != null && _anim.runtimeAnimatorController == null)
        {
#if UNITY_EDITOR
            var ctrl = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                "Assets/Entities/Alien_unequipped/AlienAnimator.controller");
            if (ctrl != null)
            {
                _anim.runtimeAnimatorController = ctrl;
                Debug.Log("[AlienController] Animator controller loaded from assets.");
            }
#endif
        }
    }

    private void Update()
    {
        HandleCursorLock();
        RotateWithMouse();
        MoveAndGravity();
        HandleWeaponSwitch();
        if (debugLog) DebugTick();
    }

    // ── Mouse rotation (horizontal only — Cinemachine handles vertical) ───────

    private void RotateWithMouse()
    {
        Vector2 d = GetMouseDelta();
        transform.Rotate(Vector3.up, d.x * mouseSensitivity, Space.World);
    }

    // ── Movement + Gravity ────────────────────────────────────────────────────

    private bool CheckGroundedRobust()
    {
        Vector3 origin = transform.position + Vector3.up * (_cc.radius + 0.05f);
        return Physics.SphereCast(origin, _cc.radius * 0.95f, Vector3.down,
                                   out _, 0.15f, ~0, QueryTriggerInteraction.Ignore);
    }

    private void MoveAndGravity()
    {
        _isGrounded = _cc.isGrounded || CheckGroundedRobust();
        if (_isGrounded)
        {
            if (_velocity.y < 0f) _velocity.y = -2f;
            _jumpInitiated = false;
        }

        // ── WASD input ────────────────────────────────────────────────────────
        float inputX = 0f, inputZ = 0f;
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.dKey.isPressed) inputX += 1f;
            if (kb.aKey.isPressed) inputX -= 1f;
            if (kb.wKey.isPressed) inputZ += 1f;
            if (kb.sKey.isPressed) inputZ -= 1f;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");
        }
#else
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
#endif

        // ── Speed tier: walk / run (LShift) / sprint (LShift + RMouse) ───────
        bool lShift   = IsHeld("lshift",  KeyCode.LeftShift);
        bool rMouse   = IsRightMouseHeld();
        bool isSprint = lShift && rMouse && inputZ > 0f;
        bool isRun    = lShift && !rMouse;

        float speedZ = inputZ < -0.01f ? backSpeed
                     : isSprint        ? sprintSpeed
                     : isRun           ? runSpeed
                     :                   walkSpeed;

        // ── Jump: Space ───────────────────────────────────────────────────────
        if (IsJustPressed("space", KeyCode.Space) && _isGrounded)
        {
            _velocity.y    = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _jumpInitiated = true;
            _anim?.SetTrigger(P_Jump);
        }

        _velocity.y += gravity * Time.deltaTime;
        if (_velocity.y < -30f) _velocity.y = -30f;

        Vector3 horizontal = transform.right   * (inputX * speedZ)
                           + transform.forward * (inputZ * speedZ);

        if (horizontal.sqrMagnitude > sprintSpeed * sprintSpeed)
            horizontal = horizontal.normalized * sprintSpeed;

        _cc.Move((horizontal + new Vector3(0f, _velocity.y, 0f)) * Time.deltaTime);

        // ── Animation parameters ──────────────────────────────────────────────
        if (_anim != null && _anim.runtimeAnimatorController != null)
        {
            bool movingF    = inputZ >  0.01f;
            bool movingB    = inputZ < -0.01f;
            bool strafeOnly = Mathf.Abs(inputX) > 0.01f && Mathf.Abs(inputZ) < 0.01f;

            float animZ = isSprint ? 3f
                        : isRun    ? 2f
                        : movingF  ? (strafeOnly ? 1f : 1f)
                        : movingB  ? -1f
                        : 0f;

            float animX = (!movingF && !movingB) ? inputX : inputX * 0.5f;

            _anim.SetFloat(P_VelX, animX, 0.1f, Time.deltaTime);
            _anim.SetFloat(P_VelZ, animZ, 0.1f, Time.deltaTime);
            _anim.SetBool(P_Grounded,  _isGrounded);
            _anim.SetBool(P_IsFalling, !_isGrounded && !_jumpInitiated);
        }
    }

    // ── Weapon switching (mouse scroll wheel) ─────────────────────────────────

    private void HandleWeaponSwitch()
    {
        float scroll = GetScrollY();
        if (scroll == 0f) return;

        int prev = _weaponIndex;
        if (scroll > 0f)
            _weaponIndex = (_weaponIndex - 1 + WeaponNames.Length) % WeaponNames.Length;
        else
            _weaponIndex = (_weaponIndex + 1) % WeaponNames.Length;

        if (_weaponIndex != prev)
            Debug.Log($"[Weapon] {WeaponNames[prev]} → {WeaponNames[_weaponIndex]}");
    }

    // ── Cursor lock ───────────────────────────────────────────────────────────

    private void HandleCursorLock()
    {
        if (IsJustPressed("escape", KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        if (IsLeftMouseJustDown() && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }

    // ── Input helpers ─────────────────────────────────────────────────────────

    private bool IsHeld(string newKey, KeyCode oldKey)
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (newKey == "lshift") return kb.leftShiftKey.isPressed;
            if (newKey == "space")  return kb.spaceKey.isPressed;
        }
#endif
        return Input.GetKey(oldKey);
    }

    private bool IsJustPressed(string newKey, KeyCode oldKey)
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (newKey == "space")  return kb.spaceKey.wasPressedThisFrame;
            if (newKey == "escape") return kb.escapeKey.wasPressedThisFrame;
        }
#endif
        return Input.GetKeyDown(oldKey);
    }

    private bool IsRightMouseHeld()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = Mouse.current;
        if (mouse != null) return mouse.rightButton.isPressed;
#endif
        return Input.GetMouseButton(1);
    }

    private bool IsLeftMouseJustDown()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = Mouse.current;
        if (mouse != null) return mouse.leftButton.wasPressedThisFrame;
#endif
        return Input.GetMouseButtonDown(0);
    }

    private Vector2 GetMouseDelta()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = Mouse.current;
        if (mouse != null) return mouse.delta.ReadValue();
#endif
        return new Vector2(Input.GetAxisRaw("Mouse X") * 10f,
                           Input.GetAxisRaw("Mouse Y") * 10f);
    }

    private float GetScrollY()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = Mouse.current;
        if (mouse != null)
        {
            float s = mouse.scroll.ReadValue().y;
            if (s > 0.1f) return 1f;
            if (s < -0.1f) return -1f;
            return 0f;
        }
#endif
        float raw = Input.GetAxis("Mouse ScrollWheel");
        if (raw > 0.01f) return 1f;
        if (raw < -0.01f) return -1f;
        return 0f;
    }

    // ── Debug ─────────────────────────────────────────────────────────────────

    private void DebugTick()
    {
        _debugTimer += Time.deltaTime;
        if (_debugTimer < debugInterval) return;
        _debugTimer = 0f;

        Vector3 posDelta = transform.position - _lastPos;
        _lastPos = transform.position;

        string animInfo = "no-anim";
        if (_anim != null && _anim.runtimeAnimatorController != null)
        {
            var s = _anim.GetCurrentAnimatorStateInfo(0);
            animInfo = $"VelX={_anim.GetFloat(P_VelX):F2} VelZ={_anim.GetFloat(P_VelZ):F2}";
        }

        Debug.Log($"[AlienDbg] pos=({transform.position.x:F1},{transform.position.z:F1}) " +
                  $"spd=({posDelta.x / debugInterval:F1},{posDelta.z / debugInterval:F1}) " +
                  $"grnd={_isGrounded} weapon={WeaponNames[_weaponIndex]} | {animInfo}");
    }
}
