using Game.Data;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class CollisionController : ILocalController
    {
        private int _bladeLayer = LayerMask.NameToLayer("Blade");
        private int _environmentLayer = LayerMask.NameToLayer("Environment");
        private int _sliceableLayer = LayerMask.NameToLayer("Sliceable");
        private int _handleLayer = LayerMask.NameToLayer("Handle");

        private GeneralConfig _generalConfig;

        public void Apply(LocalKnifeData data, float deltaTime)
        {
            if (data.UnprocessedCollision == null)
            {
                return;
            }

            var myPartLayer = data.UnprocessedCollision.Value.Item1;
            var encounteredLayer = data.UnprocessedCollision.Value.Item2;

            data.UnprocessedCollision = null;

            if (encounteredLayer == _sliceableLayer)
            {
                return;
            }

            if (myPartLayer == _handleLayer)
            {
                data.HandleHit = true;
                data.Velocity = new Vector3(_generalConfig.HorizontalVelocity, _generalConfig.YInputAcceleration);
                return;
            }

            if (myPartLayer == _bladeLayer && encounteredLayer == _environmentLayer)
            {
                if (data.FreeCutAngle > 0)
                {
                    return;
                }

                data.Velocity = Vector3.zero;
                data.Stuck = true;
                return;
            }
        }

        public CollisionController(LocalKnifeData data, GeneralConfig generalConfig)
        {
            _generalConfig = generalConfig;
            
            foreach (var collider in data.KnifeView.Colliders)
            {
                collider.CollisionTrigger += (myLayer, otherLayer) => data.UnprocessedCollision = (myLayer, otherLayer);
            }
        }
    }
}