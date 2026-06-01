using UnityEngine;

namespace _01.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerInputSo input;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float groundedStickForce = -2f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private float groundCheckOffset = 0.08f;

        private CharacterController _controller;
        private Vector3 _velocity;
        private float _lastGroundedTime;
        private float _lastJumpPressedTime;

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

            Vector2 moveInput = input.MovementKey;
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

            if (moveDirection.sqrMagnitude > 1f)
            {
                moveDirection.Normalize();
            }

            bool isGrounded = CheckGrounded();

            if (isGrounded)
            {
                _lastGroundedTime = Time.time;

                if (_velocity.y < 0f)
                {
                    _velocity.y = groundedStickForce;
                }
            }

            if (input.JumpPressed)
            {
                _lastJumpPressedTime = Time.time;
                input.ConsumeJump();
            }

            bool canUseCoyoteTime = Time.time - _lastGroundedTime <= coyoteTime;
            bool hasBufferedJump = Time.time - _lastJumpPressedTime <= jumpBufferTime;

            if (canUseCoyoteTime && hasBufferedJump)
            {
                _lastGroundedTime = -999f;
                _lastJumpPressedTime = -999f;
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            _velocity.y += gravity * Time.deltaTime;

            Vector3 horizontalVelocity = moveDirection * moveSpeed;
            Vector3 frameVelocity = horizontalVelocity + Vector3.up * _velocity.y;

            CollisionFlags flags = _controller.Move(frameVelocity * Time.deltaTime);

            if ((flags & CollisionFlags.Above) != 0 && _velocity.y > 0f)
            {
                _velocity.y = 0f;
            }
        }

        private bool CheckGrounded()
        {
            Vector3 spherePosition = transform.position + Vector3.up * groundCheckOffset;

            return Physics.CheckSphere(
                spherePosition,
                groundCheckRadius,
                groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void OnDrawGizmosSelected()
        {
            CharacterController controller = _controller != null ? _controller : GetComponent<CharacterController>();

            if (controller == null)
            {
                return;
            }

            Vector3 spherePosition = transform.position + Vector3.up * groundCheckOffset;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
        }
    }
}
