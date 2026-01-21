using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
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
        
        public List<HexInfo> GenerateMap()
        {
            var hexInfos = IMapStrategy.CreateBlankMap();
            var dictionary = IMapStrategy.CreateHexInfoDictionary(hexInfos);
            
            var islandCentres = new List<Vector2>();
            for (var i = 0; i < Random.Range(_islandCount.x, _islandCount.y); i++)
            {
                islandCentres.Add(Random.insideUnitCircle * HexGrid.GridRadius * 0.8f);
            }
            
            foreach (var (coordinate, value) in dictionary)
            {
                var worldPosition = IMapStrategy.CubicTo2DSpace(coordinate);
                var noisePosition = worldPosition * NoiseScale + _noiseOffset;
                var noise = IMapStrategy.FractalBrownianMotion(noisePosition, Octaves);
                var falloff = GetFalloff(worldPosition, islandCentres);
                falloff *= Mathf.Lerp(0.85f, 1f, noise);
                var height01 = noise * falloff;
                var height = Mathf.RoundToInt(Mathf.Lerp(MinimumHeight, _heightScale, height01));
                value.Height = Mathf.Clamp(height, MinimumHeight, HexFactory.MaxHeight);
            }
            
            return hexInfos;
        }
        
        private static float GetFalloff(Vector2 position, List<Vector2> islandCentres)
        {
            var d1 = float.MaxValue;
            var d2 = float.MaxValue;

            foreach (var center in islandCentres)
            {
                var distance = Vector2.Distance(position, center);
                if (distance < d1)
                {
                    d2 = d1;
                    d1 = distance;
                }
                else if (distance < d2)
                {
                    d2 = distance;
                }
            }

            var edge = d2 - d1;
            var t = Mathf.Clamp01(edge / (HexGrid.GridRadius * 0.3f));
            return Mathf.SmoothStep(1f, 0f, t);
        }
    }
}