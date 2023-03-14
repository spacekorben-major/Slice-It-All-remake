using Game.Data;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class RotationController : ILocalController
    {
        private GeneralConfig _config;

        private bool _wasAccelerated;

        public void Apply(LocalKnifeData data, float deltaTime)
        {
            var transform = data.KnifeView.transform;

            if (data.Stuck)
            {
                data.AngularVelocity = Vector3.zero;
                return;
            }

            // ReSharper disable once ReplaceWithSingleAssignment.True
            // Single assignment is not readable
            var accelerated = true;

            if (transform.rotation.eulerAngles.z > _config.MaxRotationAngle &&
                transform.rotation.eulerAngles.z < _config.MinRotationAngle &&
                data.FreeCutAngle == 0)
            {
                accelerated = false;
            }

            var velocityZ = accelerated ? _config.AcceleratedZRotationVelocity : _config.NormalZRotationVelocity;

            data.AngularVelocity = new Vector3(data.AngularVelocity.x, data.AngularVelocity.y, velocityZ);
        }

        public RotationController(GeneralConfig config)
        {
            _config = config;
        }
    }
}