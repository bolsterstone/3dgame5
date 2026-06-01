using _01.Scripts.Echo;
using UnityEngine;

namespace _01.Scripts.Core
{
    public class LoopManager : MonoBehaviour
    {
        public static LoopManager Instance { get; private set; }

        [SerializeField] private float loopDuration = 20f;
        [SerializeField] private Transform player;
        [SerializeField] private bool startOnAwake = true;

        private EchoObject[] _echoObjects;
        private Vector3 _playerStartPosition;
        private Quaternion _playerStartRotation;

        public float LoopTime { get; private set; }
        public int LoopCount { get; private set; } = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _echoObjects = FindObjectsByType<EchoObject>(FindObjectsSortMode.None);

            if (player != null)
            {
                _playerStartPosition = player.position;
                _playerStartRotation = player.rotation;
            }
        }

        private void Start()
        {
            if (startOnAwake)
            {
                BeginLoop();
            }
        }

        private void Update()
        {
            LoopTime += Time.deltaTime;

            if (LoopTime >= loopDuration)
            {
                EndLoop();
                BeginLoop();
            }
        }

        public void BeginLoop()
        {
            LoopTime = 0f;
            ResetPlayer();

            foreach (EchoObject echoObject in _echoObjects)
            {
                echoObject.BeginLoop();
            }
        }

        public void EndLoop()
        {
            foreach (EchoObject echoObject in _echoObjects)
            {
                echoObject.EndLoop();
            }

            LoopCount++;
        }

        private void ResetPlayer()
        {
            if (player == null)
            {
                return;
            }

            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
            }

            player.SetPositionAndRotation(_playerStartPosition, _playerStartRotation);

            if (controller != null)
            {
                controller.enabled = true;
            }
        }
    }
}
