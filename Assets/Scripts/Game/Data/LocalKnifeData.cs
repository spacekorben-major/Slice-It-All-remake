using UnityEngine;

namespace Game.Data
{
    public sealed class LocalKnifeData : KnifeData
    {
        public Vector3 Velocity;

        public Vector3 AngularVelocity;

        public bool Stuck;

        public bool HandleHit;

        public float FreeCutAngle = 0;
    }
}