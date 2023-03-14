using Game.Data;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class PlayerHorizontalMovementController : ILocalController
    {
        private GeneralConfig _generalConfig;
        public void Apply(LocalKnifeData data, float deltaTime)
        {
            if (data.Stuck)
            {
                return;
            }

            var xMod = _generalConfig.HorizontalVelocity;
            if (data.HandleHit)
            {
                xMod = -xMod;
            }

            data.Velocity = new Vector3(xMod, data.Velocity.y);
        }

        public PlayerHorizontalMovementController(GeneralConfig generalConfig)
        {
            _generalConfig = generalConfig;
        }
    }
}