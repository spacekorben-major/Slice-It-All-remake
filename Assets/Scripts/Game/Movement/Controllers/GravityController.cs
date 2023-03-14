using Game.Data;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class GravityController : ILocalController
    {
        private GeneralConfig _generalConfig;
        
        public void Apply(LocalKnifeData data, float deltaTime)
        {
            if (data.Stuck)
            {
                return;
            }

            if (data.Velocity.y <= _generalConfig.YMaxVelocity)
            {
                return;
            }

            var newYVelocity = data.Velocity.y + _generalConfig.YAcceleration * deltaTime;

            data.Velocity = new Vector3(
                data.Velocity.x,
                Mathf.Max(newYVelocity, _generalConfig.YMaxVelocity),
                data.Velocity.z
            );
        }

        public GravityController(GeneralConfig generalConfig)
        {
            _generalConfig = generalConfig;
        }
    }
}