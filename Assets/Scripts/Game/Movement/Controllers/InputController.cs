using Game.Data;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class InputController : ILocalController
    {
        private GeneralConfig _generalConfig;

        private InputMap _inputMap;

        public void Apply(LocalKnifeData data, float deltaTime)
        {
            if (_inputMap.Default.Slice.WasPerformedThisFrame())
            {
                if (data.KnifeView.transform.position.y >= _generalConfig.MaxYToJump)
                {
                    return;
                }

                data.Stuck = false;
                data.HandleHit = false;
                data.FreeCutAngle = _generalConfig.FreeCutAngle;

                data.Velocity = new Vector3(_generalConfig.HorizontalVelocity, _generalConfig.YInputAcceleration);
            }
        }

        public InputController(GeneralConfig generalConfig, InputMap inputMap)
        {
            _generalConfig = generalConfig;
            _inputMap = inputMap;
        }
    }
}