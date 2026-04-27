using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// Rotates this transform (CamPivot) around X axis for camera pitch.
/// Cinemachine3rdPersonFollow then orbits the camera around the pitched pivot.
public class CmAimController : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private float minPitch    = -40f;
    [SerializeField] private float maxPitch    =  60f;

    private float _pitch;

    private void Start()
    {
        _pitch = transform.localEulerAngles.x;
        if (_pitch > 180f) _pitch -= 360f;
    }

    private void Update()
    {
        float mouseY = GetMouseY();
        if (Mathf.Abs(mouseY) < 0.001f) return;
        _pitch = Mathf.Clamp(_pitch - mouseY * sensitivity, minPitch, maxPitch);
        transform.localEulerAngles = new Vector3(_pitch, 0f, 0f);
    }

    private float GetMouseY()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = Mouse.current;
        if (mouse != null) return mouse.delta.y.ReadValue();
#endif
        return Input.GetAxisRaw("Mouse Y") * 10f;
    }
}
