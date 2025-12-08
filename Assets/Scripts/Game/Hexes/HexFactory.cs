using System;
using Game.Grid;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Hexes
{
    public class HexFactory : IDisposable
    {
        private Material _material = Resources.Load<Material>("Materials/mat_land");

        public void Dispose()
        {
            _material = null;
        }
        
        public HexObject CreateHex(CubicCoordinate coordinate, Transform parent)
        {
            var go = new GameObject(coordinate.ToString());
            go.transform.parent = parent;
            go.transform.position = HexGrid.GetWorldPosition(coordinate);

            var hexObject = go.AddComponent<HexObject>();
            var meshObject = CreateMeshObject();
            var collider = meshObject.AddComponent<MeshCollider>();
            collider.convex = true;
            
            hexObject.Initialize(coordinate, meshObject.transform);
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

        private static Mesh CreateMesh()
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

        private static Vector3[] GetPrismVertices(float height)
        {
            var vertices = new Vector3[18];

            // top face vertices
            for (var i = 0; i < 6; i++)
            {
                vertices[i] = HexGrid.GetLocalVertexPosition(i) + Vector3.up * height;
            }

            // top of side face vertices
            for (var i = 6; i < 12; i++)
            {
                vertices[i] = HexGrid.GetLocalVertexPosition(i - 6) + Vector3.up * height;
            }

            // bottom of side face vertices
            for (var i = 12; i < 18; i++)
            {
                vertices[i] = HexGrid.GetLocalVertexPosition(i - 12);
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