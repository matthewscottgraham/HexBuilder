using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using Game.Map.Falloff;
using UnityEngine;

namespace Game.Map
{
    public class SmallIslandMap : IMapStrategy
    {
        private const float NoiseScale = 8f;
        private const int Octaves = 3;
        private const int MinimumHeight = 0;
        private const float HeightScale = 3;
        private readonly Vector2 _noiseOffset = new (Random.value * 1000, Random.value * 1000);
        private readonly IFalloff _falloff = new RadialFalloff();
        
        public List<HexInfo> GenerateMap()
        {
            var hexInfos = IMapStrategy.CreateBlankMap();
            var dictionary = IMapStrategy.CreateHexInfoDictionary(hexInfos);
            
            foreach (var (coordinate, value) in dictionary)
            {
                var noisePosition = HexGrid.CubicTo2DSpace(coordinate) * NoiseScale + _noiseOffset;
                var noise = IMapStrategy.FractalBrownianMotion(noisePosition, Octaves);
                var height01 = noise * _falloff.GetFalloffAtWorldPosition(HexGrid.GetWorldPosition(coordinate));
                var height = Mathf.RoundToInt(Mathf.Lerp(MinimumHeight, HeightScale, height01));
                value.Height = Mathf.Clamp(height, MinimumHeight, HexFactory.MaxHeight);
            }
            
            return hexInfos;
        }
    }
}