using UnityEngine;

namespace _01.Scripts.Echo
{
    [System.Serializable]
    public struct EchoFrame
    {
        public float Time;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public EchoFrame(float time, Rigidbody rb)
        {
            Time = time;
            Position = rb.position;
            Rotation = rb.rotation;
            Velocity = rb.linearVelocity;
            AngularVelocity = rb.angularVelocity;
        }
    }
}
