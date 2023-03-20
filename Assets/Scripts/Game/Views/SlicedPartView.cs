using UnityEngine;

namespace Game.Views
{
    public class SlicedPartView : MonoBehaviour
    {
        public MeshFilter MeshFilter;

        public MeshRenderer MeshRenderer;

        public MeshCollider MeshCollider;

        public Rigidbody Rigidbody;
        
        public MeshFilter InternalsMeshFilter;

        public MeshRenderer InternalsMeshRenderer;
    }
}