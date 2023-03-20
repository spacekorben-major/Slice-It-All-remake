using Game.Data;
using Game.Events;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class EnemyCollisionController : IEnemyController
    {
        private int _bladeLayer = LayerMask.NameToLayer("Blade");
        private int _sliceableLayer = LayerMask.NameToLayer("Sliceable");

        private ISignalPublisher _signalPublisher;

        public void Apply(KnifeData data)
        {
            if (data.UnprocessedCollision != null)
            {
                ProcessCollision(data);
                data.UnprocessedCollision = null;
            }
        }

        private void ProcessCollision(KnifeData data)
        {
            var contact = data.UnprocessedCollision.contacts[0];
            var myPartLayer = contact.thisCollider.gameObject.layer;
            var encounteredLayer = contact.otherCollider.gameObject.layer;

            if (myPartLayer == _bladeLayer && encounteredLayer == _sliceableLayer)
            {
                _signalPublisher.Publish(new SlicedEvent
                {
                    IsLocalPlayer = false,
                    Contact = contact
                });
            }
        }

        public EnemyCollisionController(ISignalPublisher signalPublisher)
        {
            _signalPublisher = signalPublisher;
        }
    }
}