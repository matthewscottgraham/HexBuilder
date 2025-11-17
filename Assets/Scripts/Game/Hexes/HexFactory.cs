using Game.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hexes
{
    public class HexFactory
    {
        private Material _material;
        public GameObject CreateHex(Cell cell, HexGrid grid, Transform parent)
        {
            var go = new GameObject(cell.ToString());
            go.transform.position = grid.GetHexCenter(cell.x, cell.y);
            go.transform.parent = parent;
            
            var filter = go.AddComponent<MeshFilter>();
            filter.mesh = CreateMesh(grid);

            if (_material == null) _material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.material = _material;
            
            var collider = go.AddComponent<CapsuleCollider>();
            collider.radius = grid.InnerRadius;
            collider.height = 1;

            return go;
        }
        
        private Mesh CreateMesh(HexGrid hexGrid)
        {
            var mesh = new Mesh
            {
                vertices = GetPrismVertices( hexGrid,1),
                triangles = GetPrismTriangles(),
            };
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        private Vector3[] GetPrismVertices(HexGrid hexGrid, float height)
        {
            var vertices = new Vector3[18];
            
            // top face vertices
            for (int i = 0; i < 6; i++)
            {
                vertices[i] = hexGrid.GetCornerPosition(Vector3.zero, i) + Vector3.up * height;
            }
            
            // top of side face vertices
            for(int i = 6; i < 12; i++)
            {
                vertices[i] = hexGrid.GetCornerPosition(Vector3.zero, i - 6) + Vector3.up * height;
            }
            
            // bottom of side face vertices
            for(int i = 12; i < 18; i++)
            {
                vertices[i] = hexGrid.GetCornerPosition(Vector3.zero, i - 12);
            }
            
            return vertices;
        }
        
        private int[] GetPrismTriangles()
        {
            var triangles = new int[]
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