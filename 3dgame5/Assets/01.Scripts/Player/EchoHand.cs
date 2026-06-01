using _01.Scripts.Echo;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01.Scripts.Player
{
    public class EchoHand : MonoBehaviour
    {
        [SerializeField] private PlayerInputSo input;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private LayerMask grabbableLayers = ~0;
        [SerializeField] private float grabRange = 8f;
        [SerializeField] private float holdDistance = 3f;
        [SerializeField] private float minHoldDistance = 1.2f;
        [SerializeField] private float maxHoldDistance = 7f;
        [SerializeField] private float scrollSpeed = 0.5f;
        [SerializeField] private float springForce = 60f;
        [SerializeField] private float damping = 10f;
        [SerializeField] private float throwForce = 12f;

        private Rigidbody _grabbedRb;
        private bool _wasGrabPressed;

        private void Awake()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
        }

        private void OnDisable()
        {
            Release();
        }

        private void Update()
        {
            bool isGrabPressed = input != null && input.AttackPressed;

            if (isGrabPressed && !_wasGrabPressed)
            {
                if (_grabbedRb == null)
                {
                    TryGrab();
                }
                else
                {
                    Release();
                }
            }

            _wasGrabPressed = isGrabPressed;

            if (_grabbedRb == null)
            {
                return;
            }

            AdjustHoldDistance();

            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                Throw();
            }
        }

        private void FixedUpdate()
        {
            if (_grabbedRb == null || playerCamera == null)
            {
                return;
            }

            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
            Vector3 direction = targetPosition - _grabbedRb.position;
            Vector3 force = direction * springForce - _grabbedRb.linearVelocity * damping;

            _grabbedRb.AddForce(force, ForceMode.Acceleration);
        }

        private void TryGrab()
        {
            if (playerCamera == null)
            {
                return;
            }

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (!Physics.Raycast(ray, out RaycastHit hit, grabRange, grabbableLayers, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            if (!hit.rigidbody)
            {
                return;
            }

            _grabbedRb = hit.rigidbody;
            
            if (_grabbedRb.TryGetComponent(out EchoObject echoObject))
            {
                echoObject.InterruptReplay();
            }

            _grabbedRb.isKinematic = false;
            _grabbedRb.useGravity = false;
            holdDistance = Mathf.Clamp(hit.distance, minHoldDistance, maxHoldDistance);
        }

        private void AdjustHoldDistance()
        {
            if (Mouse.current == null)
            {
                return;
            }

            float scrollY = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scrollY) < 0.01f)
            {
                return;
            }

            holdDistance = Mathf.Clamp(
                holdDistance + Mathf.Sign(scrollY) * scrollSpeed,
                minHoldDistance,
                maxHoldDistance);
        }

        private void Release()
        {
            if (_grabbedRb == null)
            {
                return;
            }

            _grabbedRb.useGravity = true;
            _grabbedRb = null;
        }

        private void Throw()
        {
            Rigidbody rb = _grabbedRb;
            Release();

            if (rb != null && playerCamera != null)
            {
                rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
            }
        }
    }
}
