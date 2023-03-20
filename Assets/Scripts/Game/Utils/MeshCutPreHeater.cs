using UnityEngine;

namespace Game.Utils
{
    public static class MeshCutPreHeater
    {
        public static void KickMono(MeshCut meshCut)
        {
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

                uv = new[]
                {
                    Vector2.zero, Vector2.zero, Vector2.zero
                },

                colors = new[]
                {
                    Color.black, Color.black, Color.black
                }
            };

            meshCut.Slice(mesh, plane, out var meshPositive, out var meshNegative,
                out var positiveInsides, out var negativeInsides);
        }
    }
}