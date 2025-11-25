using System;
using Game.Grid;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Hexes
{
    public class HexFactory : IDisposable
    {
        private readonly Material _material = Resources.Load<Material>("Materials/mat_land");
        private readonly HexGrid _hexGrid;
        private readonly IObjectPool<GameObject> _pool;

        public HexFactory(HexGrid hexGrid)
        {
            _hexGrid = hexGrid;
            _pool = new ObjectPool<GameObject>(CreateMeshObject, null, null, null, true, 50);
        }

        public void Dispose()
        {
            _pool.Clear();
        }
        
        public HexObject CreateHex(Cell cell, Transform parent)
        {
            var go = new GameObject(cell.ToString());
            go.transform.position = _hexGrid.GetHexCenter(cell.X, cell.Y);
            go.transform.parent = parent;

            var collider = go.AddComponent<CapsuleCollider>();
            collider.radius = _hexGrid.InnerRadius;
            collider.height = 1;

            var hexObject = go.AddComponent<HexObject>();
            hexObject.Initialize(cell, collider, _pool.Get().transform);
            
            return hexObject;
        }

        private GameObject CreateMeshObject()
        {
            var hexMesh = new GameObject("HexMesh");
            var filter = hexMesh.AddComponent<MeshFilter>();
            filter.mesh = CreateMesh();

            var renderer = hexMesh.AddComponent<MeshRenderer>();
            renderer.material = _material;
            return hexMesh;
        }

        private Mesh CreateMesh()
        {
            var mesh = new Mesh
            {
                vertices = GetPrismVertices(1),
                triangles = GetPrismTriangles()
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private Vector3[] GetPrismVertices(float height)
        {
            var vertices = new Vector3[18];

            // top face vertices
            for (var i = 0; i < 6; i++) vertices[i] = _hexGrid.GetCornerPosition(Vector3.zero, i) + Vector3.up * height;

            // top of side face vertices
            for (var i = 6; i < 12; i++)
                vertices[i] = _hexGrid.GetCornerPosition(Vector3.zero, i - 6) + Vector3.up * height;

            // bottom of side face vertices
            for (var i = 12; i < 18; i++) vertices[i] = _hexGrid.GetCornerPosition(Vector3.zero, i - 12);

            return vertices;
        }

        private static int[] GetPrismTriangles()
        {
            var triangles = new[]
            {
                // Top:
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5,
                // Sides:
                12, 7, 6,
                13, 7, 12,
                13, 8, 7,
                14, 8, 13,
                14, 9, 8,
                15, 9, 14,
                15, 10, 9,
                16, 10, 15,
                16, 11, 10,
                17, 11, 16,
                17, 6, 11,
                12, 6, 17
            };

            return triangles;
        }
    }
}