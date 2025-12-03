using System;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class HexFactory : IDisposable
    {
        private Material _material = Resources.Load<Material>("Materials/mat_land");
        private HexGrid _hexGrid;

        public HexFactory(HexGrid hexGrid)
        {
            _hexGrid = hexGrid;
        }

        public void Dispose()
        {
            _hexGrid = null;
            _material = null;
        }
        
        public HexObject CreateHex(Coordinate2 coordinate, Transform parent)
        {
            var go = new GameObject(coordinate.ToString());
            go.transform.parent = parent;
            go.transform.position = _hexGrid.GetFacePosition(coordinate);
            
            var collider = go.AddComponent<CapsuleCollider>();
            collider.radius = _hexGrid.InnerRadius;
            collider.height = 1;

            var hexObject = go.AddComponent<HexObject>();
            hexObject.Initialize(coordinate, collider, CreateMeshObject(coordinate).transform);
            
            return hexObject;
        }

        private GameObject CreateMeshObject(Coordinate2 coordinate)
        {
            var hexMesh = new GameObject("HexMesh");
            var filter = hexMesh.AddComponent<MeshFilter>();
            filter.mesh = CreateMesh(coordinate);

            var renderer = hexMesh.AddComponent<MeshRenderer>();
            renderer.material = _material;
            return hexMesh;
        }

        private Mesh CreateMesh(Coordinate2 coordinate)
        {
            var mesh = new Mesh
            {
                vertices = GetPrismVertices(1, coordinate),
                triangles = GetPrismTriangles()
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private Vector3[] GetPrismVertices(float height, Coordinate2 coordinate)
        {
            var vertices = new Vector3[18];

            // top face vertices
            for (var i = 0; i < 6; i++)
            {
                vertices[i] = _hexGrid.GetLocalVertexPosition(i) + Vector3.up * height;
            }

            // top of side face vertices
            for (var i = 6; i < 12; i++)
            {
                vertices[i] = _hexGrid.GetLocalVertexPosition(i - 6) + Vector3.up * height;
            }

            // bottom of side face vertices
            for (var i = 12; i < 18; i++)
            {
                vertices[i] = _hexGrid.GetLocalVertexPosition(i - 12);
            }

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