using UnityEngine;
using UnityEngine.InputSystem;

namespace _01.Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerInputSo", menuName = "So/Player/PlayerInput", order = 0)]
    public class PlayerInputSo : ScriptableObject, Controls.IPlayerActions
    {
        private Controls _controls;

        public Vector2 MovementKey { get; private set; }
        public Vector2 LookDelta { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool AttackPressed { get; private set; }
        public bool InteractPressed { get; private set; }

        private void OnEnable()
        {
            if (_controls != null)
                return;

            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            if (_controls == null)
                return;

            _controls.Player.RemoveCallbacks(this);
            _controls.Player.Disable();

            _controls = null;

            MovementKey = Vector2.zero;
            LookDelta = Vector2.zero;
            JumpPressed = false;
            AttackPressed = false;
            InteractPressed = false;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementKey = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookDelta = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            AttackPressed = context.performed;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            InteractPressed = context.performed;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                JumpPressed = true;
        }

        public void ConsumeJump()
        {
            JumpPressed = false;
        }
    }
}