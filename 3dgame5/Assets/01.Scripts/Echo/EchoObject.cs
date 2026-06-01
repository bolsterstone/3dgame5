using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts.Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class EchoObject : MonoBehaviour
    {
        [SerializeField] private float recordInterval = 0.05f;
        [SerializeField] private LayerMask interruptLayers = ~0;

        private readonly List<EchoFrame> _recordedFrames = new();
        private readonly List<EchoFrame> _replayFrames = new();
        private Rigidbody _rb;
        private float _nextRecordTime;
        private int _replayIndex;
        private bool _isRecording;
        private bool _isReplaying;
        private bool _isPhysicsOverride;

        public bool HasReplay => _replayFrames.Count > 1;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float loopTime = Core.LoopManager.Instance != null ? Core.LoopManager.Instance.LoopTime : 0f;

            if (_isRecording)
            {
                Record(loopTime);
            }

            if (_isReplaying && !_isPhysicsOverride)
            {
                Replay(loopTime);
            }
        }

        public void BeginLoop()
        {
            _recordedFrames.Clear();
            _nextRecordTime = 0f;
            _isRecording = true;
            _isPhysicsOverride = false;

            if (HasReplay)
            {
                _isReplaying = true;
                _replayIndex = 0;
                _rb.isKinematic = true;
            }
            else
            {
                _isReplaying = false;
                _rb.isKinematic = false;
            }
        }

        public void EndLoop()
        {
            _isRecording = false;
            _isReplaying = false;
            _isPhysicsOverride = false;

            _replayFrames.Clear();
            _replayFrames.AddRange(_recordedFrames);
            _rb.isKinematic = false;
        }

        private void Record(float loopTime)
        {
            if (loopTime < _nextRecordTime)
            {
                return;
            }

            _recordedFrames.Add(new EchoFrame(loopTime, _rb));
            _nextRecordTime = loopTime + recordInterval;
        }

        private void Replay(float loopTime)
        {
            while (_replayIndex < _replayFrames.Count - 2 && _replayFrames[_replayIndex + 1].Time <= loopTime)
            {
                _replayIndex++;
            }

            EchoFrame current = _replayFrames[_replayIndex];
            EchoFrame next = _replayFrames[Mathf.Min(_replayIndex + 1, _replayFrames.Count - 1)];
            float duration = Mathf.Max(next.Time - current.Time, 0.0001f);
            float t = Mathf.Clamp01((loopTime - current.Time) / duration);

            _rb.MovePosition(Vector3.Lerp(current.Position, next.Position, t));
            _rb.MoveRotation(Quaternion.Slerp(current.Rotation, next.Rotation, t));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isReplaying || _isPhysicsOverride)
            {
                return;
            }

            if ((interruptLayers.value & (1 << collision.gameObject.layer)) == 0)
            {
                return;
            }

            SwitchToPhysics();
        }

        private void SwitchToPhysics()
        {
            EchoFrame frame = _replayFrames[Mathf.Clamp(_replayIndex, 0, _replayFrames.Count - 1)];

            _isPhysicsOverride = true;
            _rb.isKinematic = false;
            _rb.linearVelocity = frame.Velocity;
            _rb.angularVelocity = frame.AngularVelocity;
        }

        public void InterruptReplay()
        {
            if (!_isReplaying || _isPhysicsOverride)
            {
                return;
            }

            SwitchToPhysics();
        }
    }
}
