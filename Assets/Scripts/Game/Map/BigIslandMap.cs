using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Map
{
    public class BigIslandMap : IMapStrategy
    {
        private const float NoiseScale = 2f;
        private const int Octaves = 5;
        private const int MinimumHeight = 0;
        private readonly float _heightScale = HexFactory.MaxHeight;
        private readonly Vector2 _noiseOffset = new (Random.value * 1000, Random.value * 1000);
        
        public List<HexInfo> GenerateMap()
        {
            var hexInfos = IMapStrategy.CreateBlankMap();
            var dictionary = IMapStrategy.CreateHexInfoDictionary(hexInfos);
            
            foreach (var (coordinate, value) in dictionary)
            {
                var noisePosition = IMapStrategy.CubicTo2DSpace(coordinate) * NoiseScale + _noiseOffset;
                var noise = IMapStrategy.FractalBrownianMotion(noisePosition, Octaves);
                var falloff = GetFalloff(coordinate);
                var height01 = noise * falloff;
                var height = Mathf.RoundToInt(Mathf.Lerp(MinimumHeight, _heightScale, height01));
                value.Height = Mathf.Clamp(height, MinimumHeight, HexFactory.MaxHeight);
            }
            
            return hexInfos;
        }
        
        private static float GetFalloff(CubicCoordinate coordinate)
        {
            var t = (float)IMapStrategy.DistanceFromCentre(coordinate) / HexGrid.GridRadius;
            t = Mathf.Clamp01(t);
            // This is a S curve style falloff
            return 1f / (1f + Mathf.Exp(6f * (t - 0.6f)));
        }
    }
}