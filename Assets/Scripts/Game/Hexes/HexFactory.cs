using System;
using System.Collections.Generic;
using App.Utils;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class HexFactory : MonoBehaviour, IDisposable
    {
        private readonly Color _highlightColour = Color.white;
        private static readonly int ShaderColourID = Shader.PropertyToID("_Color");
        private static Dictionary<int, Material> _materials = new();
        private GameObject _hexTopPrefab;
        private GameObject _hexSidePrefab;
        
        public static int MaxHeight => 6;
        public static int WaterHeight => 2;
        public static Material HighlightMaterial { get; private set; }
        
        private static Color[] GetLandColours()
        {
            return new[]
            {
                GetColour("#4B726E"),
                GetColour("#8CABA1"),
                GetColour("#D2C9A5"),
                GetColour("#B3A555"),
                GetColour("#77743B"),
                GetColour("#4D4539"),
                GetColour("#4B3D44")
            };
        }

        private static Color GetColour(string hexCode)
        {
            ColorUtility.TryParseHtmlString(hexCode, out var color);
            return color;
        }
        
        private void Awake()
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
            
            _hexTopPrefab = Resources.Load<GameObject>("Meshes/hexTop");
            _hexSidePrefab = Resources.Load<GameObject>("Meshes/hexSide");
            
            HighlightMaterial = Instantiate(material);
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

            var hexTop = Instantiate(_hexTopPrefab, parent);
            var hexSides = Instantiate(_hexSidePrefab, parent);
            var hexSidesMeshObject = hexSides.transform.GetChild(0).gameObject;
            var meshCollider = hexSidesMeshObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            
            hexObject.Initialize(coordinate, hexSides.transform, hexTop.transform);
            return hexObject;
        }

        public static Material GetMaterialForHeight(int height)
        {
            height = Mathf.Clamp(height, 0, MaxHeight);
            return _materials[height];
        }
    }
}