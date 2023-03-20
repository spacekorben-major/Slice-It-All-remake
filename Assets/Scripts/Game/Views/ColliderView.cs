using System;
using UnityEngine;

namespace Game.Views
{
    public sealed class ColliderView : MonoBehaviour
    {
        public Action<Collision> CollisionTrigger = _ => { };

        private void OnCollisionEnter(Collision collision)
        {
            CollisionTrigger(collision);
        }
    }
}