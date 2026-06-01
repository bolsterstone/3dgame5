using UnityEngine;
using UnityEngine.InputSystem;

namespace _01.Scripts.Player
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 0.15f;
        
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpPower = 8f;

        private CharacterController _characterController;
        private float xRotation;
        private float verticalVelocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            if (cameraTransform == null)
                cameraTransform = Camera.main.transform;
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
            lookAction.action.Enable();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            lookAction.action.Disable();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            Look();
            Move();
        }

        private void Look()
        {
            Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

            float mouseX = lookInput.x * mouseSensitivity;
            float mouseY = lookInput.y * mouseSensitivity;
            
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
            transform.Rotate(Vector3.up * mouseX);
        }

        private void Move()
        {
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

            Vector3 move =
                transform.right * moveInput.x +
                transform.forward * moveInput.y;

            if (_characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;

            move.y = verticalVelocity;

            _characterController.Move(move * moveSpeed * Time.deltaTime);
        }
    }
}