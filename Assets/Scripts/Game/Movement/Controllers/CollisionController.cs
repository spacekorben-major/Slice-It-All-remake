using Game.Data;
using Game.Events;
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

        private ISignalPublisher _signalPublisher;

        public void Apply(LocalKnifeData data, float deltaTime)
        {
            if (data.UnprocessedCollision == null)
            {
                return;
            }

            var myPartLayer = data.UnprocessedCollision.contacts[0].thisCollider.gameObject.layer;
            var encounteredLayer = data.UnprocessedCollision.contacts[0].otherCollider.gameObject.layer;

            if (encounteredLayer == _sliceableLayer)
            {
                _signalPublisher.Publish(new SlicedEvent
                {
                    Contact = data.UnprocessedCollision.contacts[0]
                });

                data.UnprocessedCollision = null;
                return;
            }

            data.UnprocessedCollision = null;

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

        public CollisionController(LocalKnifeData data, GeneralConfig generalConfig, ISignalPublisher signalPublisher)
        {
            _generalConfig = generalConfig;

            _signalPublisher = signalPublisher;

            foreach (var collider in data.KnifeView.Colliders)
            {
                collider.CollisionTrigger += collision => data.UnprocessedCollision = collision;
            }
        }
    }
}