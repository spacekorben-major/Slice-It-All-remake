using Game.Data;
using Game.Events;
using Game.Views;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class CollisionController : ILocalController
    {
        private int _bladeLayer = LayerMask.NameToLayer("Blade");
        private int _gameEndLayer = LayerMask.NameToLayer("GameEnd");
        private int _sliceableLayer = LayerMask.NameToLayer("Sliceable");
        private int _handleLayer = LayerMask.NameToLayer("Handle");

        private GeneralConfig _generalConfig;

        private ISignalPublisher _signalPublisher;

        public void Apply(LocalKnifeData data, float deltaTime)
        {
            if (data.UnprocessedCollision != null)
            {
                ProcessCollision(data);
                data.UnprocessedCollision = null;
            }
        }

        private void ProcessCollision(LocalKnifeData data)
        {
            var contact = data.UnprocessedCollision.contacts[0];
            var myPartLayer = contact.thisCollider.gameObject.layer;
            var encounteredLayer = contact.otherCollider.gameObject.layer;

            if (myPartLayer == _handleLayer)
            {
                data.HandleHit = true;
                data.Stuck = false;
                data.Velocity = new Vector3(data.Velocity.x, _generalConfig.YInputAcceleration);
                return;
            }

            if (myPartLayer == _bladeLayer)
            {
                if (encounteredLayer == _sliceableLayer)
                {
                    _signalPublisher.Publish(new SlicedEvent
                    {
                        IsLocalPlayer = true,
                        Contact = contact
                    });
                    return;
                }

                if (encounteredLayer == _gameEndLayer)
                {
                    data.PlayerDataSync.Finished.Value = true;
                    var mod = contact.otherCollider.gameObject.GetComponent<EndGameView>().Mod;
                    _signalPublisher.Publish(new PlayerFinished(mod));
                    data.Velocity = Vector3.zero;
                    data.Stuck = true;
                    return;
                }

                if (data.FreeCutAngle > 0)
                {
                    return;
                }

                data.Velocity = Vector3.zero;
                data.Stuck = true;
            }
        }

        public CollisionController(GeneralConfig generalConfig, ISignalPublisher signalPublisher)
        {
            _generalConfig = generalConfig;

            _signalPublisher = signalPublisher;
        }
    }
}