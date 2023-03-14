using Game.Data;
using UnityEngine;

namespace Game.Movement.Controllers
{
    public class VelocityApplierController : ILocalController
    {
        public void Apply(LocalKnifeData data, float deltaTime)
        {
            var transform = data.KnifeView.transform;

            if (data.FreeCutAngle > 0)
            {
                data.FreeCutAngle = Mathf.Max(data.FreeCutAngle - Mathf.Abs(data.AngularVelocity.z * deltaTime), 0);
            }

            transform.position += data.Velocity * deltaTime;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + data.AngularVelocity * deltaTime);
        }
    }
}