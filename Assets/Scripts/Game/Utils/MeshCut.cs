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

        private List<int> _originalTrianglesCache;
        private List<Vector3> _originalVerticesCache;
        private List<Vector2> _originalUVCache;

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

            _intersectionVertices = new List<Vector3>(kAllocationSize);
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

            originalMesh.GetTriangles(_originalTrianglesCache, 0);
            originalMesh.GetVertices(_originalVerticesCache);
            originalMesh.GetUVs(0, _originalUVCache);

            for (int i = 0; i < _originalTrianglesCache.Count; i+=3)
            {
                var vert0 = _originalVerticesCache[_originalTrianglesCache[i]];
                var vert1 = _originalVerticesCache[_originalTrianglesCache[i + 1]];
                var vert2 = _originalVerticesCache[_originalTrianglesCache[i + 2]];

                var uv0 = _originalUVCache[_originalTrianglesCache[i]];
                var uv1 = _originalUVCache[_originalTrianglesCache[i + 1]];
                var uv2 = _originalUVCache[_originalTrianglesCache[i + 2]];

                var vert0Side = cutPlane.GetSide(vert0);
                var vert1Side = cutPlane.GetSide(vert1);
                var vert2Side = cutPlane.GetSide(vert2);

                if ((vert0Side == vert1Side)&&(vert0Side == vert2Side))
                {
                    var verticesList = vert0Side ? _verticesPositive : _verticesNegative;
                    var trianglesList = vert0Side ? _trianglesPositive : _trianglesNegative;
                    var uvList = vert0Side ? _uvPositive : _uvNegative;

                    AddPoint(vert0, uv0, verticesList, trianglesList, uvList);
                    AddPoint(vert1, uv1, verticesList, trianglesList, uvList);
                    AddPoint(vert2, uv2, verticesList, trianglesList, uvList);

                    continue;
                }

                if (vert0Side == vert1Side) // vert2 is solo
                {
                    var ray02 = new Ray(vert0, vert2 - vert0);
                    cutPlane.Raycast(ray02, out var distance);

                    var intersection02 = ray02.GetPoint(distance);
                    var uv02 = Vector2.Lerp(uv0, uv2, distance / (vert2 - vert0).magnitude);

                    var ray12 = new Ray(vert1, vert2 - vert1);
                    cutPlane.Raycast(ray12, out distance);

                    var intersection12 = ray12.GetPoint(distance);
                    var uv12 = Vector2.Lerp(uv1, uv2, distance / (vert2 - vert1).magnitude);

                    var verticesListSolo = vert2Side ? _verticesPositive : _verticesNegative;
                    var trianglesListSolo = vert2Side ? _trianglesPositive : _trianglesNegative;
                    var uvListSolo = vert2Side ? _uvPositive : _uvNegative;

                    var verticesListDuo = vert2Side ? _verticesNegative : _verticesPositive;
                    var trianglesListDuo = vert2Side ? _trianglesNegative : _trianglesPositive;
                    var uvListDuo = vert2Side ? _uvNegative : _uvPositive;

                    AddPoint(intersection02, uv02, verticesListSolo, trianglesListSolo, uvListSolo);
                    AddPoint(intersection12, uv12, verticesListSolo, trianglesListSolo, uvListSolo);
                    AddPoint(vert2, uv2, verticesListSolo, trianglesListSolo, uvListSolo);

                    AddPoint(vert0, uv0, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(vert1, uv1, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(intersection02, uv02, verticesListDuo, trianglesListDuo, uvListDuo);

                    AddPoint(vert1, uv1, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(intersection12, uv12, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(intersection02, uv02, verticesListDuo, trianglesListDuo, uvListDuo);

                    _intersectionVertices.Add(intersection02);
                    _intersectionVertices.Add(intersection12);
                }
                else if (vert0Side == vert2Side) // vert1 is solo
                {
                    var ray01 = new Ray(vert0, vert1 - vert0);
                    cutPlane.Raycast(ray01, out var distance);

                    var intersection01 = ray01.GetPoint(distance);
                    var uv01 = Vector2.Lerp(uv0, uv1, distance / (vert1 - vert0).magnitude);

                    var ray12 = new Ray(vert1, vert2 - vert1);
                    cutPlane.Raycast(ray12, out distance);

                    var intersection12 = ray12.GetPoint(distance);
                    var uv12 = Vector2.Lerp(uv1, uv2, distance / (vert2 - vert1).magnitude);

                    var verticesListSolo = vert1Side ? _verticesPositive : _verticesNegative;
                    var trianglesListSolo = vert1Side ? _trianglesPositive : _trianglesNegative;
                    var uvListSolo = vert1Side ? _uvPositive : _uvNegative;

                    var verticesListDuo = vert1Side ? _verticesNegative : _verticesPositive;
                    var trianglesListDuo = vert1Side ? _trianglesNegative : _trianglesPositive;
                    var uvListDuo = vert1Side ? _uvNegative : _uvPositive;

                    AddPoint(intersection01, uv01, verticesListSolo, trianglesListSolo, uvListSolo);
                    AddPoint(vert1, uv1, verticesListSolo, trianglesListSolo, uvListSolo);
                    AddPoint(intersection12, uv12, verticesListSolo, trianglesListSolo, uvListSolo);

                    AddPoint(vert0, uv0, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(intersection01, uv01, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(vert2, uv2, verticesListDuo, trianglesListDuo, uvListDuo);

                    AddPoint(intersection01, uv01, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(intersection12, uv12, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(vert2, uv2, verticesListDuo, trianglesListDuo, uvListDuo);

                    _intersectionVertices.Add(intersection01);
                    _intersectionVertices.Add(intersection12);
                }
                else // vert0 is solo
                {
                    var ray01 = new Ray(vert0, vert1 - vert0);
                    cutPlane.Raycast(ray01, out var distance);

                    var intersection01 = ray01.GetPoint(distance);
                    var uv01 = Vector2.Lerp(uv0, uv1, distance / (vert1 - vert0).magnitude);

                    var ray02 = new Ray(vert0, vert2 - vert0);
                    cutPlane.Raycast(ray02, out distance);

                    var intersection02 = ray02.GetPoint(distance);
                    var uv02 = Vector2.Lerp(uv0, uv2, distance / (vert2 - vert0).magnitude);

                    var verticesListSolo = vert0Side ? _verticesPositive : _verticesNegative;
                    var trianglesListSolo = vert0Side ? _trianglesPositive : _trianglesNegative;
                    var uvListSolo = vert0Side ? _uvPositive : _uvNegative;

                    var verticesListDuo = vert0Side ? _verticesNegative : _verticesPositive;
                    var trianglesListDuo = vert0Side ? _trianglesNegative : _trianglesPositive;
                    var uvListDuo = vert0Side ? _uvNegative : _uvPositive;

                    AddPoint(vert0, uv0, verticesListSolo, trianglesListSolo, uvListSolo);
                    AddPoint(intersection01, uv01, verticesListSolo, trianglesListSolo, uvListSolo);
                    AddPoint(intersection02, uv02, verticesListSolo, trianglesListSolo, uvListSolo);

                    AddPoint(intersection01, uv01, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(vert1, uv1, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(intersection02, uv02, verticesListDuo, trianglesListDuo, uvListDuo);

                    AddPoint(intersection02, uv02, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(vert1, uv1, verticesListDuo, trianglesListDuo, uvListDuo);
                    AddPoint(vert2, uv2, verticesListDuo, trianglesListDuo, uvListDuo);

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
            
            meshPositive.RecalculateNormals();
            meshNegative.RecalculateNormals();
            positiveInsides.RecalculateNormals();
            negativeInsides.RecalculateNormals();
        }

        private void AddPoint(Vector3 point, Vector2 uvCoordinate, List<Vector3> verticesList, List<int> trianglesList,
            List<Vector2> uvList)
        {
            trianglesList.Add(verticesList.Count);
            verticesList.Add(point);
            uvList.Add(uvCoordinate);
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