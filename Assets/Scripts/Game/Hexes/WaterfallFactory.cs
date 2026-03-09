using System.Collections.Generic;
using App.Services;
using App.VFX;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class WaterfallFactory
    {
        private const string WaterfallVfxID = "waterfallVFX";
        private const float WaterfallWidth = 0.333f;
        private const float WaterfallDepth = 0.1f;
        private readonly Material _waterfallMaterial = Resources.Load<Material>("Materials/mat_river");
        private readonly Dictionary<int, Mesh> _meshes = new ();
        
        public WaterfallFactory()
        {
            ServiceLocator.Instance.Get<VFXController>().RegisterVFX(WaterfallVfxID);
        }
        
        public Transform CreateWaterFall(HexObject hexA, HexObject hexB)
        {
            var top = hexA.Height > hexB.Height ? hexA : hexB;
            var bottom = hexA.Height > hexB.Height ? hexB : hexA;
            
            var height = top.Height - bottom.Height;
            
            var waterfall = new GameObject("Waterfall");
            
            var filter = waterfall.AddComponent<MeshFilter>();
            filter.sharedMesh = GetWaterfallMesh(height);
            
            var renderer = waterfall.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = _waterfallMaterial;
            
            var vfx = ServiceLocator.Instance.Get<VFXController>().GetPersistentVFX(WaterfallVfxID);
            vfx.transform.SetParent(waterfall.transform);
            vfx.transform.localPosition = new Vector3(-2f, -height, 0);
            
            return waterfall.transform;
        }

        private Mesh GetWaterfallMesh(int height)
        {
            if (_meshes.ContainsKey(height)) return _meshes[height];

            var vertex = HexGrid.GetLocalVertexPosition(0);
            var mesh = new Mesh
            {
                name = $"Waterfall_{height}",
                vertices = new Vector3[]
                {
                    new (-1.74f + WaterfallDepth, 0, -1 * WaterfallWidth),
                    new (-1.74f + WaterfallDepth, 0, 1 * WaterfallWidth),
                    new (-1.74f + WaterfallDepth, -height, -1 * WaterfallWidth),
                    new (-1.74f + WaterfallDepth, -height, 1 * WaterfallWidth),
                    new (-1.74f - WaterfallDepth, 0, -1 * WaterfallWidth),
                    new (-1.74f - WaterfallDepth, 0, 1 * WaterfallWidth),
                    new (-1.74f - WaterfallDepth, -height, -1 * WaterfallWidth),
                    new (-1.74f - WaterfallDepth, -height, 1 * WaterfallWidth)
                },
                triangles = new []
                {
                    0, 1, 2, 
                    3, 2, 1,
                    4, 0, 6,
                    2, 6, 0,
                    5, 4, 7,
                    6, 7, 4,
                    1, 5, 3,
                    7, 3, 5,
                    4, 5, 0,
                    1, 0, 5,
                    2, 3, 6,
                    7, 6, 3
                },
                uv = new Vector2[]
                {
                    new (0, 1),
                    new (1, 1),
                    new (0, 0),
                    new (1, 0),
                    new (0, 1),
                    new (1, 1),
                    new (0, 0),
                    new (1, 0)
                }
            };

            _meshes.Add(height, mesh);
            return mesh;
        }
    }
}
