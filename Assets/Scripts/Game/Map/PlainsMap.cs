using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Map
{
    public class PlainsMap : IMapStrategy
    {
        private const float NoiseScale = 25f;
        private const int Octaves = 1;
        private const int MinimumHeight = 2;
        private readonly float _heightScale = 3;
        private readonly Vector2 _noiseOffset = new (Random.value * 1000, Random.value * 1000);
        
        public List<HexInfo> GenerateMap()
        {
            var hexInfos = IMapStrategy.CreateBlankMap();
            var dictionary = IMapStrategy.CreateHexInfoDictionary(hexInfos);
            
            foreach (var (coordinate, value) in dictionary)
            {
                var noisePosition = HexGrid.CubicTo2DSpace(coordinate) * NoiseScale + _noiseOffset;
                var noise = IMapStrategy.FractalBrownianMotion(noisePosition, Octaves) * 0.8f;
                var height = Mathf.RoundToInt(Mathf.Lerp(MinimumHeight, _heightScale, noise));
                value.Height = Mathf.Clamp(height, MinimumHeight, HexFactory.MaxHeight);
            }
            
            return hexInfos;
        }
    }
}