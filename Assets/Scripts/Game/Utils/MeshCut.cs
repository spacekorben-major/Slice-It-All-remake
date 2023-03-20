using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Game.Utils
{
    public class MeshCut : IStartable
    {
        private List<Vector3> _verticesPositive;
        private List<Vector3> _verticesNegative;

        private List<int> _trianglesPositive;
        private List<int> _trianglesNegative;
        
        private List<Vector3> _verticesInsidePositive;
        private List<Vector3> _verticesInsideNegative;

        private List<int> _trianglesInsidePositive;
        private List<int> _trianglesInsideNegative;

        private List<Vector2> _uvPositive;
        private List<Vector2> _uvNegative;

        private List<Color> _colorPositive;
        private List<Color> _colorNegative;

        private List<int> _originalTrianglesCache;
        private List<Vector3> _originalVerticesCache;
        private List<Vector2> _originalUVCache;
        private List<Color> _originalColorCache;

        private List<Vector3> _intersectionVertices;

        private const int kAllocationSize = 2048;

        public void Start()
        {
            _verticesPositive = new List<Vector3>(kAllocationSize);
            _verticesNegative = new List<Vector3>(kAllocationSize);
            _trianglesPositive = new List<int>(kAllocationSize);
            _trianglesNegative = new List<int>(kAllocationSize);
            _uvPositive = new List<Vector2>(kAllocationSize);
            _uvNegative = new List<Vector2>(kAllocationSize);

            _verticesInsidePositive = new List<Vector3>(kAllocationSize);
            _verticesInsideNegative = new List<Vector3>(kAllocationSize);
            _trianglesInsidePositive = new List<int>(kAllocationSize);
            _trianglesInsideNegative = new List<int>(kAllocationSize);

            _originalTrianglesCache = new List<int>(kAllocationSize);
            _originalVerticesCache = new List<Vector3>(kAllocationSize);
            _originalUVCache = new List<Vector2>(kAllocationSize);
            _originalColorCache = new List<Color>(kAllocationSize);

            _intersectionVertices = new List<Vector3>(kAllocationSize);

            _colorPositive = new List<Color>(kAllocationSize);
            _colorNegative = new List<Color>(kAllocationSize);
        }

        public void Slice(Mesh originalMesh, Plane cutPlane, out Mesh meshPositive, out Mesh meshNegative,
            out Mesh positiveInsides, out Mesh negativeInsides)
        {
            _verticesPositive.Clear();
            _verticesNegative.Clear();
            _trianglesPositive.Clear();
            _trianglesNegative.Clear();
            _uvPositive.Clear();
            _uvNegative.Clear();

            _originalTrianglesCache.Clear();
            _originalVerticesCache.Clear();
            _originalUVCache.Clear();
            _intersectionVertices.Clear();
            
            _trianglesInsideNegative.Clear();
            _trianglesInsidePositive.Clear();
            _verticesInsideNegative.Clear();
            _verticesInsidePositive.Clear();
            
            _colorNegative.Clear();
            _colorPositive.Clear();

            originalMesh.GetTriangles(_originalTrianglesCache, 0);
            originalMesh.GetVertices(_originalVerticesCache);
            originalMesh.GetUVs(0, _originalUVCache);
            originalMesh.GetColors(_originalColorCache);

            var colorEnabled = _originalColorCache.Count > 0;
            var uvEnabled = _originalUVCache.Count > 0;

            for (int i = 0; i < _originalTrianglesCache.Count; i+=3)
            {
                var indexV0 = _originalTrianglesCache[i];
                var indexV1 = _originalTrianglesCache[i+1];
                var indexV2 = _originalTrianglesCache[i+2];
                
                var vert0 = _originalVerticesCache[indexV0];
                var vert1 = _originalVerticesCache[indexV1];
                var vert2 = _originalVerticesCache[indexV2];

                var uv0 = Vector2.zero;
                var uv1 = Vector2.zero;
                var uv2 = Vector2.zero;

                if (uvEnabled)
                {
                    uv0 = _originalUVCache[indexV0];
                    uv1 = _originalUVCache[indexV1];
                    uv2 = _originalUVCache[indexV2];
                }

                var color0 = Color.black;
                var color1 = Color.black;
                var color2 = Color.black;

                if (colorEnabled)
                {
                    color0 = _originalColorCache[indexV0];
                    color1 = _originalColorCache[indexV1];
                    color2 = _originalColorCache[indexV2];
                }

                var vert0Side = cutPlane.GetSide(vert0);
                var vert1Side = cutPlane.GetSide(vert1);
                var vert2Side = cutPlane.GetSide(vert2);

                if ((vert0Side == vert1Side)&&(vert0Side == vert2Side))
                {
                    AddPoint(indexV0, vert0Side, uvEnabled, colorEnabled);
                    AddPoint(indexV1, vert0Side, uvEnabled, colorEnabled);
                    AddPoint(indexV2, vert0Side, uvEnabled, colorEnabled);

                    continue;
                }

                if (vert0Side == vert1Side) // vert2 is solo
                {
                    var ray02 = new Ray(vert0, vert2 - vert0);
                    cutPlane.Raycast(ray02, out var distance);

                    var intersection02 = ray02.GetPoint(distance);
                    var t = distance / (vert2 - vert0).magnitude;
                    var uv02 = Vector2.Lerp(uv0, uv2, t);
                    var color02 = Color.Lerp(color0, color2, t);

                    var ray12 = new Ray(vert1, vert2 - vert1);
                    cutPlane.Raycast(ray12, out distance);

                    var intersection12 = ray12.GetPoint(distance);
                    t = distance / (vert2 - vert1).magnitude;
                    var uv12 = Vector2.Lerp(uv1, uv2, t);
                    var color12 = Color.Lerp(color1, color2, t);

                    AddPoint(intersection02, uv02, color02, vert2Side);
                    AddPoint(intersection12, uv12, color12, vert2Side);
                    AddPoint(indexV2, vert2Side, uvEnabled, colorEnabled);

                    AddPoint(indexV0, vert0Side, uvEnabled, colorEnabled);
                    AddPoint(indexV1, vert0Side, uvEnabled, colorEnabled);
                    AddPoint(intersection02, uv02, color02, vert0Side);

                    AddPoint(indexV1, vert0Side, uvEnabled, colorEnabled);
                    AddPoint(intersection12, uv12, color12, vert0Side);
                    AddPoint(intersection02, uv02, color02, vert0Side);

                    _intersectionVertices.Add(intersection02);
                    _intersectionVertices.Add(intersection12);
                }
                else if (vert0Side == vert2Side) // vert1 is solo
                {
                    var ray01 = new Ray(vert0, vert1 - vert0);
                    cutPlane.Raycast(ray01, out var distance);

                    var intersection01 = ray01.GetPoint(distance);
                    var t = distance / (vert1 - vert0).magnitude;
                    var uv01 = Vector2.Lerp(uv0, uv1, t);
                    var color01 = Color.Lerp(color0, color1, t);

                    var ray12 = new Ray(vert1, vert2 - vert1);
                    cutPlane.Raycast(ray12, out distance);

                    var intersection12 = ray12.GetPoint(distance);
                    t = distance / (vert2 - vert1).magnitude;
                    var uv12 = Vector2.Lerp(uv1, uv2, t);
                    var color12 = Color.Lerp(color1, color2, t);

                    AddPoint(intersection01, uv01, color01, vert1Side);
                    AddPoint(indexV1, vert1Side, uvEnabled, colorEnabled);
                    AddPoint(intersection12, uv12, color12, vert1Side);

                    AddPoint(indexV0, vert0Side, uvEnabled, colorEnabled);
                    AddPoint(intersection01, uv01, color01, vert0Side);
                    AddPoint(indexV2, vert0Side, uvEnabled, colorEnabled);

                    AddPoint(intersection01, uv01, color01, vert0Side);
                    AddPoint(intersection12, uv12, color12, vert0Side);
                    AddPoint(indexV2, vert0Side, uvEnabled, colorEnabled);

                    _intersectionVertices.Add(intersection01);
                    _intersectionVertices.Add(intersection12);
                }
                else // vert0 is solo
                {
                    var ray01 = new Ray(vert0, vert1 - vert0);
                    cutPlane.Raycast(ray01, out var distance);

                    var intersection01 = ray01.GetPoint(distance);
                    var t = distance / (vert1 - vert0).magnitude;
                    var uv01 = Vector2.Lerp(uv0, uv1, t);
                    var color01 = Color.Lerp(color0, color1, t);

                    var ray02 = new Ray(vert0, vert2 - vert0);
                    cutPlane.Raycast(ray02, out distance);

                    var intersection02 = ray02.GetPoint(distance);
                    t = distance / (vert2 - vert0).magnitude;
                    var uv02 = Vector2.Lerp(uv0, uv2, t);
                    var color02 = Color.Lerp(color0, color2, t);

                    AddPoint(indexV0, vert0Side, uvEnabled, colorEnabled);
                    AddPoint(intersection01, uv01, color01, vert0Side);
                    AddPoint(intersection02, uv02, color02, vert0Side);

                    AddPoint(intersection01, uv01, color01, vert1Side);
                    AddPoint(indexV1, vert1Side, uvEnabled, colorEnabled);
                    AddPoint(intersection02, uv02, color02, vert1Side);

                    AddPoint(intersection02, uv02, color02, vert1Side);
                    AddPoint(indexV1, vert1Side, uvEnabled, colorEnabled);
                    AddPoint(indexV2, vert1Side, uvEnabled, colorEnabled);

                    _intersectionVertices.Add(intersection01);
                    _intersectionVertices.Add(intersection02);
                }
            }

            Vector3 centroid = Vector3.zero;
            foreach (Vector3 point in _intersectionVertices)
            {
                centroid += point;
            }
            centroid /= _intersectionVertices.Count;

            _intersectionVertices.Sort((pointA, pointB) =>
                Vector3.SignedAngle(pointA - centroid, pointB - centroid, cutPlane.normal)
                    .CompareTo(Vector3.SignedAngle(pointB - centroid, pointA - centroid, cutPlane.normal)));

            GenerateInsideMeshData(centroid);

            meshPositive = new Mesh
            {
                vertices = _verticesPositive.ToArray(),
                triangles = _trianglesPositive.ToArray(),
                uv = _uvPositive.ToArray()
            };

            positiveInsides = new Mesh
            {
                vertices = _verticesInsidePositive.ToArray(),
                triangles = _trianglesInsidePositive.ToArray()
            };

            meshNegative = new Mesh
            {
                vertices = _verticesNegative.ToArray(),
                triangles = _trianglesNegative.ToArray(),
                uv = _uvNegative.ToArray(),
            };

            negativeInsides = new Mesh
            {
                vertices = _verticesInsideNegative.ToArray(),
                triangles = _trianglesInsideNegative.ToArray()
            };

            if (colorEnabled)
            {
                meshPositive.colors = _colorPositive.ToArray();
                meshNegative.colors = _colorNegative.ToArray();
            }

            if (uvEnabled)
            {
                meshPositive.uv = _uvPositive.ToArray();
                meshNegative.uv = _uvNegative.ToArray();
            }

            meshPositive.RecalculateTangents();
            meshNegative.RecalculateTangents();
            meshPositive.RecalculateNormals();
            meshNegative.RecalculateNormals();
            positiveInsides.RecalculateNormals();
            negativeInsides.RecalculateNormals();
        }

        private void AddPoint(int index, bool positive, bool uvAllowed, bool colorsAllowed)
        {
            if (positive)
            {
                _trianglesPositive.Add(_verticesPositive.Count);
                _verticesPositive.Add(_originalVerticesCache[index]);

                if (uvAllowed)
                {
                    _uvPositive.Add(_originalUVCache[index]);
                }

                if (colorsAllowed)
                {
                    _colorPositive.Add(_originalColorCache[index]);
                }
            }
            else
            {
                _trianglesNegative.Add(_verticesNegative.Count);
                _verticesNegative.Add(_originalVerticesCache[index]);

                if (uvAllowed)
                {
                    _uvNegative.Add(_originalUVCache[index]);
                }

                if (colorsAllowed)
                {
                    _colorNegative.Add(_originalColorCache[index]);
                }
            }
        }

        private void AddPoint(Vector3 point, Vector2 uv, Color color, bool positive)
        {
            if (positive)
            {
                _trianglesPositive.Add(_verticesPositive.Count);
                _verticesPositive.Add(point);
                _uvPositive.Add(uv);
                _colorPositive.Add(color);
            }
            else
            {
                _trianglesNegative.Add(_verticesNegative.Count);
                _verticesNegative.Add(point);
                _uvNegative.Add(uv);
                _colorNegative.Add(color);
            }
        }

        // This will work for simpler meshes, but we might want to improve it later
        private void GenerateInsideMeshData(Vector3 centroid)
        {
            for (int i = 0; i < _intersectionVertices.Count; i++)
            {
                _trianglesInsideNegative.Add(_verticesInsideNegative.Count);
                _verticesInsideNegative.Add(_intersectionVertices[i]);
                _trianglesInsideNegative.Add(_verticesInsideNegative.Count);
                _verticesInsideNegative.Add(centroid);
                _trianglesInsideNegative.Add(_verticesInsideNegative.Count);

                if (i == _intersectionVertices.Count - 1)
                {
                    _verticesInsideNegative.Add(_intersectionVertices[0]);
                    break;
                }
                _verticesInsideNegative.Add(_intersectionVertices[i+1]);
            }

            for (int i = _intersectionVertices.Count - 1; i >= 0; i--)
            {
                _trianglesInsidePositive.Add(_verticesInsidePositive.Count);
                _verticesInsidePositive.Add(_intersectionVertices[i]);
                _trianglesInsidePositive.Add(_verticesInsidePositive.Count);
                _verticesInsidePositive.Add(centroid);
                _trianglesInsidePositive.Add(_verticesInsidePositive.Count);

                if (i == 0)
                {
                    _verticesInsidePositive.Add(_intersectionVertices[^1]);
                    break;
                }
                _verticesInsidePositive.Add(_intersectionVertices[i-1]);
            }
        }
    }
}