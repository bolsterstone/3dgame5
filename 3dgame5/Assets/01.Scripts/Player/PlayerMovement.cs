using _01.Scripts.Player;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerInputSo input;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;

    private CharacterController _controller;
    private Vector3 _velocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (input == null)
        {
            return;
        }

        Move();
        ApplyGravityAndJump();
    }

    private void Move()
    {
        Vector2 moveInput = input.MovementKey;
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        _controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void ApplyGravityAndJump()
    {
        if (_controller.isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }

        if (_controller.isGrounded && input.JumpPressed)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        input.ConsumeJump();
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
