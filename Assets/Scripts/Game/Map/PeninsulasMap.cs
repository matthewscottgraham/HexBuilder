using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using Game.Map.Falloff;
using UnityEngine;

namespace Game.Map
{
    public class PeninsulasMap : IMapStrategy
    {
        private const float NoiseScale = 0.6f;
        private const int Octaves = 3;
        private const int MinimumHeight = 0;
        private readonly float _heightScale = 4;
        private readonly Vector2 _noiseOffset = new (Random.value * 1000, Random.value * 1000);
        private readonly Vector2Int _islandCount = new (2, 4);
        private readonly IFalloff _falloff = new VoronoiFalloff(3,6);
        public List<HexInfo> GenerateMap()
        {
            var hexInfos = IMapStrategy.CreateBlankMap();
            var dictionary = IMapStrategy.CreateHexInfoDictionary(hexInfos);
            
            foreach (var (coordinate, value) in dictionary)
            {
                var worldPosition = HexGrid.CubicTo2DSpace(coordinate);
                var noisePosition = worldPosition * NoiseScale + _noiseOffset;
                var noise = IMapStrategy.FractalBrownianMotion(noisePosition, Octaves);
                var height01 = noise * _falloff.GetFalloffAtWorldPosition(HexGrid.GetWorldPosition(coordinate));
                var height = Mathf.RoundToInt(Mathf.Lerp(MinimumHeight, _heightScale, height01));
                value.Height = Mathf.Clamp(height, MinimumHeight, HexFactory.MaxHeight);
            }
            
            return hexInfos;
        }
    }
}