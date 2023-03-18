using System.Diagnostics;
using Game.Events;
using Game.Utils;
using UnityEngine;
using VContainer.Unity;
using Debug = UnityEngine.Debug;

namespace Game.Environment
{
    public class SlicingService : IStartable
    {
        private ISignalBus _signalBus;

        private MeshCut _meshCut;

        public void Start()
        {
            _signalBus.Subscribe<SlicedEvent>(this, OnSliceAttempt);
            _signalBus.Subscribe<StartGameEvent>(this, OnStartGame);
        }

        private void OnStartGame(StartGameEvent obj)
        {
            var timer = Stopwatch.StartNew();
            var plane = new Plane(Vector3.forward, Vector3.zero);
            var mesh = new Mesh
            {
                vertices = new[]
                {
                    new Vector3(-1, 0, -1),
                    new Vector3(1, 0, 1),
                    new Vector3(1, 0, -1)
                },

                triangles = new[]
                {
                    0, 1, 2
                },

                uv = new []
                {
                    Vector2.zero, Vector2.zero, Vector2.zero
                }
            };

            _meshCut.Slice(mesh, plane, out var meshPositive, out var meshNegative,
                out var positiveInsides, out var negativeInsides);
            timer.Stop();
            Debug.Log($"slice {timer.ElapsedMilliseconds.ToString()}");
        }

        private void OnSliceAttempt(SlicedEvent obj)
        {
            GameObject target = obj.Contact.otherCollider.gameObject;
            Vector3 localContactPoint = target.transform.InverseTransformPoint(obj.Contact.point);

            // Create a plane using the contact point's position and the x-axis as the normal
            Plane cuttingPlane = new Plane(target.transform.forward, localContactPoint);

            var targetMesh = target.GetComponent<MeshFilter>().mesh;
            var targetMaterial = target.GetComponent<MeshRenderer>().material;

            var timer = Stopwatch.StartNew();
            _meshCut.Slice(targetMesh, cuttingPlane, out var meshPositive, out var meshNegative,
                out var positiveInsides, out var negativeInsides);
            timer.Stop();
            Debug.Log($"slice {timer.ElapsedMilliseconds.ToString()}");

            target.gameObject.SetActive(false);

            GameObject positiveMeshObject = new GameObject
            {
                layer = 0,
                transform =
                {
                    position = target.transform.position,
                    rotation = target.transform.rotation,
                    localScale = target.transform.localScale
                }
            };

            MeshFilter positiveMeshFilter = positiveMeshObject.AddComponent<MeshFilter>();
            positiveMeshFilter.mesh = meshPositive;

            MeshRenderer positiveMeshRenderer = positiveMeshObject.AddComponent<MeshRenderer>();
            positiveMeshRenderer.material = targetMaterial;

            GameObject positiveMeshInsidesObject = new GameObject();
            positiveMeshInsidesObject.transform.SetParent(positiveMeshObject.transform);
            positiveMeshInsidesObject.layer = 0;
            positiveMeshInsidesObject.transform.localPosition = Vector3.zero;
            positiveMeshInsidesObject.transform.localScale = Vector3.one;

            MeshFilter positiveInsidesMeshFilter = positiveMeshInsidesObject.AddComponent<MeshFilter>();
            positiveInsidesMeshFilter.mesh = positiveInsides;

            MeshRenderer positiveInsidesMeshRenderer = positiveMeshInsidesObject.AddComponent<MeshRenderer>();

            GameObject negativeMeshObject = new GameObject
            {
                layer = 0,
                transform =
                {
                    position = target.transform.position,
                    rotation = target.transform.rotation,
                    localScale = target.transform.localScale
                }
            };

            MeshFilter negativeMeshFilter = negativeMeshObject.AddComponent<MeshFilter>();
            negativeMeshFilter.mesh = meshNegative;
            MeshRenderer negativeMeshRenderer = negativeMeshObject.AddComponent<MeshRenderer>();
            negativeMeshRenderer.material = targetMaterial;

            GameObject negativeMeshInsidesObject = new GameObject();
            negativeMeshInsidesObject.transform.SetParent(negativeMeshObject.transform);
            negativeMeshInsidesObject.layer = 0;
            negativeMeshInsidesObject.transform.localPosition = Vector3.zero;
            negativeMeshInsidesObject.transform.localScale = Vector3.one;

            MeshFilter negativeInsidesMeshFilter = negativeMeshInsidesObject.AddComponent<MeshFilter>();
            negativeInsidesMeshFilter.mesh = negativeInsides;

            MeshRenderer negativeInsidesMeshRenderer = negativeMeshInsidesObject.AddComponent<MeshRenderer>();
        }

        public SlicingService(ISignalBus signalBus, MeshCut meshCut)
        {
            _signalBus = signalBus;
            _meshCut = meshCut;
        }
    }
}