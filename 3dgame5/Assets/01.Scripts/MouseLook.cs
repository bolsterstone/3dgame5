using _01.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private PlayerInputSo input;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float sensitivity = 0.12f;
    [SerializeField] private Vector3 eyeOffset = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private bool lockCursor = true;
    [SerializeField] private Key unlockKey = Key.Escape;

    private float _pitch;

    private void OnEnable()
    {
        ApplyCursorLock();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ApplyCursorLock();
        }
    }

    private void Update()
    {
        UpdateCursorLock();

        if (input == null || playerBody == null)
        {
            return;
        }

        transform.position = playerBody.position + eyeOffset;
        Vector2 look = input.LookDelta * sensitivity;

        _pitch -= look.y;
        _pitch = Mathf.Clamp(_pitch, -85f, 85f);

        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        playerBody.Rotate(Vector3.up * look.x);
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ApplyCursorLock();
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            ApplyCursorLock();
        }
    }

    private void ApplyCursorLock()
    {
        if (!lockCursor)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
