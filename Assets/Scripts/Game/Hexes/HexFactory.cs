using System;
using System.Collections.Generic;
using App.Utils;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class HexFactory : IDisposable
    {
        private readonly Color _highlightColour = Color.white;
        private static readonly int ShaderColourID = Shader.PropertyToID("_Color");
        public static int MaxHeight => 6;
        
        private static Dictionary<int, Material> _materials = new();
        public static Material HighlightMaterial { get; private set; }
        
        private static Color[] GetLandColours()
        {
            return new Color[]
            {
                GetColour("#D4C63D"),
                GetColour("#3DD4CE"),
                GetColour("#D4C63D"),
                GetColour("#A6D44F"),
                GetColour("#BCA64B"),
                GetColour("#968E62"),
                GetColour("#82695B")
            };
        }

        private static Color GetColour(string hexCode)
        {
            ColorUtility.TryParseHtmlString(hexCode, out var color);
            return color;
        }
        
        public HexFactory()
        {
            _materials = new Dictionary<int, Material>();
            var landColours = GetLandColours();
            var material = Resources.Load<Material>("Materials/mat_land");
            for (var i = 0; i < MaxHeight + 1; i++)
            {
                var newMat = UnityEngine.Object.Instantiate(material);
                newMat.SetColor(ShaderColourID, landColours[i]);
                _materials.Add(i, newMat);
            }
            HighlightMaterial = UnityEngine.Object.Instantiate(material);
            HighlightMaterial.SetColor(ShaderColourID, _highlightColour);
        }

        public void Dispose()
        {
            _materials = null;
        }
        
        public HexObject CreateHex(CubicCoordinate coordinate, Transform parent)
        {
            var hexObject = parent.gameObject.AddChild<HexObject>(coordinate.ToString());
            hexObject.transform.position = HexGrid.GetWorldPosition(coordinate);
            
            var meshObject = CreateMeshObject();
            var collider = meshObject.AddComponent<MeshCollider>();
            collider.convex = true;
            
            hexObject.Initialize(coordinate, meshObject.transform);
            return hexObject;
        }

        public static Material GetMaterialForHeight(int height)
        {
            height = Mathf.Clamp(height, 0, MaxHeight);
            return _materials[height];
        }
        
        private GameObject CreateMeshObject()
        {
            var hexMesh = new GameObject("HexMesh");
            var filter = hexMesh.AddComponent<MeshFilter>();
            filter.mesh = CreateMesh();

            var renderer = hexMesh.AddComponent<MeshRenderer>();
            renderer.material = _materials[0];
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