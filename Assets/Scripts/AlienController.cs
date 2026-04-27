using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class AlienController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed   = 2.5f;
    [SerializeField] private float runSpeed    = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float backSpeed   = 2f;
    [SerializeField] private float strafeSpeed = 2.5f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.4f;
    [SerializeField] private float gravity    = -20f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool debugLog = false;
    [SerializeField] private float debugInterval = 0.2f;

    private CharacterController _cc;
    private Animator            _anim;
    private Vector3             _velocity;
    private bool                _isGrounded;
    private float               _debugTimer;
    private Vector3             _lastPos;
    private Transform           _probeBoneHips;
    private Transform           _probeBoneLFoot;
    private Vector3             _lastHipsLocal;
    private Vector3             _lastLFootLocal;

    private static readonly int P_VelX      = Animator.StringToHash("VelocityX");
    private static readonly int P_VelZ      = Animator.StringToHash("VelocityZ");
    private static readonly int P_Grounded  = Animator.StringToHash("IsGrounded");
    private static readonly int P_Jump      = Animator.StringToHash("Jump");
    private static readonly int P_IsFalling = Animator.StringToHash("IsFalling");

    private bool _jumpInitiated;

    private void Awake()
    {
        _cc   = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            if (_probeBoneHips == null && t.name == "mixamorig:Hips")        _probeBoneHips  = t;
            if (_probeBoneLFoot == null && t.name == "mixamorig:LeftFoot")   _probeBoneLFoot = t;
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
            else
                Debug.LogWarning("[AlienController] AnimatorController not found — animations won't play.");
#endif
        }
    }

    private void Update()
    {
        RotateWithMouse();
        MoveAndGravity();
        if (debugLog) DebugTick();
    }

    // ── Horizontal rotation only — Cinemachine handles vertical/camera ──────

    private void RotateWithMouse()
    {
        Vector2 d = GetMouseDelta();
        transform.Rotate(Vector3.up, d.x * mouseSensitivity, Space.World);
    }

    // ── Input helpers ────────────────────────────────────────────────────────

    private bool GetKey(string newKey, KeyCode oldKey)
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (newKey == "space")  return kb.spaceKey.isPressed;
            if (newKey == "escape") return kb.escapeKey.wasPressedThisFrame;
        }
#endif
        return Input.GetKey(oldKey);
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

    private bool LeftMouseDown()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = Mouse.current;
        if (mouse != null) return mouse.leftButton.wasPressedThisFrame;
#endif
        return Input.GetMouseButtonDown(0);
    }

    // ── Movement + Gravity ───────────────────────────────────────────────────

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

        bool jumpPressed = false;
#if ENABLE_INPUT_SYSTEM
        if (kb != null) jumpPressed = kb.leftShiftKey.wasPressedThisFrame;
        else            jumpPressed = Input.GetKeyDown(KeyCode.LeftShift);
#else
        jumpPressed = Input.GetKeyDown(KeyCode.LeftShift);
#endif
        if (jumpPressed && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _jumpInitiated = true;
            _anim?.SetTrigger(P_Jump);
        }

        _velocity.y += gravity * Time.deltaTime;
        if (_velocity.y < -30f) _velocity.y = -30f;

        bool sprint     = GetKey("space", KeyCode.Space) && inputZ > 0f;
        bool movingF    = inputZ > 0.01f;
        bool movingB    = inputZ < -0.01f;
        bool strafeOnly = Mathf.Abs(inputX) > 0.01f && Mathf.Abs(inputZ) < 0.01f;

        float speedZ = movingB ? backSpeed
                     : sprint  ? sprintSpeed
                     : movingF ? runSpeed
                     : 0f;

        Vector3 horizontal = transform.right   * (inputX * strafeSpeed)
                           + transform.forward * (inputZ * speedZ);

        if (horizontal.sqrMagnitude > sprintSpeed * sprintSpeed)
            horizontal = horizontal.normalized * sprintSpeed;

        _cc.Move((horizontal + new Vector3(0, _velocity.y, 0)) * Time.deltaTime);

        if (_anim != null && _anim.runtimeAnimatorController != null)
        {
            float animZ = sprint  ? 3f
                        : movingF ? (strafeOnly ? 1f : 2f)
                        : movingB ? -1f
                        : 0f;
            float animX = (!movingF && !movingB) ? inputX : inputX * 0.5f;
            _anim.SetFloat(P_VelX, animX);
            _anim.SetFloat(P_VelZ, animZ);
            _anim.SetBool(P_Grounded, _isGrounded);
            _anim.SetBool(P_IsFalling, !_isGrounded && !_jumpInitiated);
        }
    }

    // ── Cursor lock ──────────────────────────────────────────────────────────

    private void LateUpdate()
    {
        if (GetKey("escape", KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        if (LeftMouseDown() && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }

    // ── Debug ────────────────────────────────────────────────────────────────

    private void DebugTick()
    {
        _debugTimer += Time.deltaTime;
        if (_debugTimer < debugInterval) return;
        _debugTimer = 0f;

        Vector3 posDelta = transform.position - _lastPos;
        _lastPos = transform.position;

        string stateInfo = "no-anim";
        if (_anim != null && _anim.runtimeAnimatorController != null)
        {
            var s = _anim.GetCurrentAnimatorStateInfo(0);
            stateInfo = $"state={s.fullPathHash:X} nt={s.normalizedTime:F2} VelX={_anim.GetFloat(P_VelX):F2} VelZ={_anim.GetFloat(P_VelZ):F2}";
        }

        Debug.Log($"[AlienDbg] pos=({transform.position.x:F2},{transform.position.y:F2},{transform.position.z:F2}) " +
                  $"dPos/s=({posDelta.x / debugInterval:F2},{posDelta.z / debugInterval:F2}) " +
                  $"grounded={_isGrounded} velY={_velocity.y:F2} | {stateInfo}");
    }
}
