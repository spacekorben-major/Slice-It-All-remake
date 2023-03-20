using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GeneralConfig", order = 1)]
    public sealed class GeneralConfig : ScriptableObject
    {
        public Vector3 StartingPosition;

        public Vector3 StartingRotation;

        public float YAcceleration = -1;

        public float YMaxVelocity = -1;

        public float XAdjust = 1f;

        public float ZAdjust = 1f;

        public float HorizontalVelocity = 1f;

        public float YInputAcceleration = 2f;

        public float FreeCutAngle = 90;

        public float NormalZRotationVelocity = 0.5f;

        public float AcceleratedZRotationVelocity = 2f;

        public float MaxRotationAngle = 340f;

        public float MinRotationAngle = 190f;

        public float MaxYToJump = 2.5f;
    }
}