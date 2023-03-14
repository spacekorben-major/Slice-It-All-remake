using Game.Data;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class PositionAdjustEnemyController : IEnemyController
    {
        private GeneralConfig _generalConfig;
        public void Apply(KnifeData data)
        {
            var knifeTransform = data.KnifeView.transform;
            var networkTransform = data.PlayerDataSync.transform;
            var basePosition = networkTransform.position;
            var baseRotation = networkTransform.rotation;

            knifeTransform.position = new Vector3
            {
                x = basePosition.x + _generalConfig.XAdjust,
                y = basePosition.y,
                z = _generalConfig.ZAdjust
            };
            
            knifeTransform.rotation = baseRotation;
        }

        public PositionAdjustEnemyController(GeneralConfig generalConfig)
        {
            _generalConfig = generalConfig;
        }
    }
}