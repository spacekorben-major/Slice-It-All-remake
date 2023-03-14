using System;
using UnityEngine;

namespace Game.Views
{
    public sealed class ColliderView : MonoBehaviour
    {
        public Action<int, int> CollisionTrigger = (_, _) => {};

        private void OnTriggerEnter(Collider other)
        {
            CollisionTrigger(gameObject.layer, other.gameObject.layer);
        }
    }
}