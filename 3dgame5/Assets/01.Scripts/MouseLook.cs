using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 0.12f;
    [SerializeField] private Vector3 eyeOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private Key unlockKey = Key.Escape;

    private float _pitch;
    private bool _cursorUnlocked;

    private void Start()
    {
        ApplyCursorLock();
        Application.targetFrameRate = 60;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !_cursorUnlocked)
            ApplyCursorLock();
    }

    private void Update()
    {
        UpdateCursorLock();

        if (playerBody == null)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 look = Mouse.current.delta.ReadValue() * sensitivity;

        _pitch -= look.y;
        _pitch = Mathf.Clamp(_pitch, -85f, 85f);

        playerBody.Rotate(Vector3.up * look.x);
    }

    private void LateUpdate()
    {
        if (playerBody == null)
            return;

        transform.position = playerBody.position + eyeOffset;
        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void UpdateCursorLock()
    {
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (Keyboard.current != null && Keyboard.current[unlockKey].wasPressedThisFrame)
        {
            _cursorUnlocked = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (_cursorUnlocked && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _cursorUnlocked = false;
            ApplyCursorLock();
        }
    }

    private void ApplyCursorLock()
    {
        if (!lockCursor)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}