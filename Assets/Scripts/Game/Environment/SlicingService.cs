using System.Collections.Generic;
using Game.Data;
using Game.Events;
using Game.Utils;
using Game.Views;
using UnityEngine;
using VContainer.Unity;

namespace Game.Environment
{
    public sealed class SlicingService : IStartable
    {
        private ISignalBus _signalBus;

        private MeshCut _meshCut;

        private PrefabMap _prefabMap;

        private List<GameObject> _spawnedObjects = new List<GameObject>();

        public void Start()
        {
            _signalBus.Subscribe<SlicedEvent>(this, OnSliceAttempt);
            _signalBus.Subscribe<ResetGame>(this, OnGameReset);
        }

        private void OnGameReset(ResetGame obj)
        {
            foreach (var spawnedObject in _spawnedObjects)
            {
                GameObject.Destroy(spawnedObject);
            }

            _spawnedObjects.Clear();
        }

        private void OnSliceAttempt(SlicedEvent obj)
        {
            GameObject target = obj.Contact.otherCollider.gameObject;
            Vector3 localContactPoint = target.transform.InverseTransformPoint(obj.Contact.point);

            // Create a plane using the contact point's position and the x-axis as the normal
            Plane cuttingPlane = new Plane(target.transform.forward, localContactPoint);

            var targetMesh = target.GetComponent<MeshFilter>().mesh;
            var targetMaterial = target.GetComponent<MeshRenderer>().material;

            _meshCut.Slice(targetMesh, cuttingPlane, out var meshPositive, out var meshNegative,
                out var positiveInsides, out var negativeInsides);

            target.gameObject.SetActive(false);
            
            _spawnedObjects.Add(target.gameObject);

            SpawnPart(target, meshPositive, targetMaterial, positiveInsides);
            SpawnPart(target, meshNegative, targetMaterial, negativeInsides);
        }

        private void SpawnPart(GameObject target, Mesh mesh, Material material, Mesh insideMesh)
        {
            var slicedPart = GameObject.Instantiate<SlicedPartView>(_prefabMap.SlicedPartView);

            _spawnedObjects.Add(slicedPart.gameObject);

            slicedPart.gameObject.layer = 0;
            
            var transform = slicedPart.transform;
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
            transform.localScale = target.transform.localScale;

            slicedPart.MeshFilter.mesh = mesh;
            slicedPart.MeshRenderer.material = material;
            slicedPart.MeshCollider.sharedMesh = mesh;
            slicedPart.MeshCollider.convex = true;

            slicedPart.InternalsMeshFilter.mesh = insideMesh;
        }

        public SlicingService(ISignalBus signalBus, MeshCut meshCut, PrefabMap prefabMap)
        {
            _signalBus = signalBus;
            _meshCut = meshCut;
            _prefabMap = prefabMap;
        }
    }
}