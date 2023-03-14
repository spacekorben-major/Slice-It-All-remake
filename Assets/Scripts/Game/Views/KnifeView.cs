using UnityEngine;

namespace Game.Views
{
    public sealed class KnifeView : MonoBehaviour
    {
        [SerializeField]
        private ColliderView[] _colliders;

        public ColliderView[] Colliders => _colliders;

        private void OnValidate()
        {
            _colliders ??= GetComponentsInChildren<ColliderView>();
        }
    }
}