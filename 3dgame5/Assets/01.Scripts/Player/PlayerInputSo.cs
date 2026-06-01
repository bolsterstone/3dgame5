using UnityEngine;
using UnityEngine.InputSystem;

namespace _01.Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerInputSo", menuName = "So/Player/PlayerInput", order = 0)]
    public class PlayerInputSo : ScriptableObject,Controls.IPlayerActions
    {
        [SerializeField] private LayerMask whatIsGround;
        private Controls _controls;
        private Camera _mainCam;
        
        public Vector2 MovementKey { get; private set; }
        public void OnMove(InputAction.CallbackContext context)
        {
            MovementKey = context.ReadValue<Vector2>(); 
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}