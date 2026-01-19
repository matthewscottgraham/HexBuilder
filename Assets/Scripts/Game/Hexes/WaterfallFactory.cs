using System.Collections.Generic;
using App.Utils;
using App.VFX;
using Game.Grid;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Hexes
{
    public class WaterfallFactory
    {
        private const float WaterfallWidth = 0.333f;
        private readonly VisualEffectAsset _visualEffectAsset = Resources.Load<VisualEffectAsset>("VFX/waterfallVFX");
        private readonly Material _waterfallMaterial = Resources.Load<Material>("Materials/mat_river");
        private readonly Dictionary<int, Mesh> _meshes = new Dictionary<int, Mesh>();
        
        public Transform CreateWaterFall(HexObject hexA, HexObject hexB, int edgeA, int edgeB)
        {
            var height = Mathf.Abs(hexA.Height - hexB.Height);
            
            var waterfall = new GameObject("Waterfall");
            
            var filter = waterfall.AddComponent<MeshFilter>();
            filter.sharedMesh = GetWaterfallMesh(height);
            
            var renderer = waterfall.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = _waterfallMaterial;
            
            var vfx = waterfall.AddChild<VisualEffect>();
            vfx.visualEffectAsset = _visualEffectAsset;
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
                    new (-1.74f, 0, -1 * WaterfallWidth),
                    new (-1.74f, 0, 1 * WaterfallWidth),
                    new (-1.74f, -height, -1 * WaterfallWidth),
                    new (-1.74f, -height, 1 * WaterfallWidth),
                },
                triangles = new [] { 1, 0, 2, 3, 1, 2 },
                uv = new Vector2[]
                {
                    new (0, 1),
                    new (1, 1),
                    new (0, 0),
                    new (1, 0),
                }
            };

            _meshes.Add(height, mesh);
            return mesh;
        }
    }
}
