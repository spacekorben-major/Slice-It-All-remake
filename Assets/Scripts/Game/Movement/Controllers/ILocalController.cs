using Game.Data;

namespace Game.Movement.Controllers
{
    public interface ILocalController
    {
        public void Apply(LocalKnifeData data, float deltaTime);
    }
}