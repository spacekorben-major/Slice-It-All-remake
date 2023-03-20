using Game.Data;

namespace Game.Movement.Controllers
{
    public class PositionTranslationController : ILocalController
    {
        public void Apply(LocalKnifeData data, float deltaTime)
        {
            var transform = data.KnifeView.transform;
            var targetTransform = data.Transform.transform;

            var position = transform.position;
            var rotation = transform.rotation;

            targetTransform.position = position;
            targetTransform.rotation = rotation;
        }
    }
}