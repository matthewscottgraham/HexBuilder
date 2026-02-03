using System.Collections.Generic;
using System.Linq;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using Game.Map.Falloff;
using Game.Map.Noise;
using UnityEngine;

namespace Game.Map
{
    public class MapStrategy
    {
        protected Vector2Int HeightRange;
        
        protected INoise Noise;
        protected IFalloff Falloff;
        
        public virtual List<HexInfo> GenerateMap()
        {
            var hexInfos = CreateBlankMap();
            var dictionary = CreateHexInfoDictionary(hexInfos);
            
            foreach (var (coordinate, hexInfo) in dictionary)
            {
                var worldPosition = HexGrid.GetWorldPosition(coordinate);
                var noise = Noise.GetValueAtWorldPosition(worldPosition);
                var height01 = noise * Falloff.GetFalloffAtWorldPosition(worldPosition);
                
                var height = Mathf.RoundToInt(Mathf.Lerp(HeightRange.x, HeightRange.y, height01));
                hexInfo.Height = Mathf.Clamp(height, HeightRange.x, HexFactory.MaxHeight);

                AddRandomFaceFeature(hexInfo);
            }
            
            return hexInfos;
        }

        protected static List<HexInfo> CreateBlankMap()
        {
            var hexInfos = new List<HexInfo>();
            var radius = HexGrid.GridRadius;
            for (var x = -radius; x <= radius; x++)
            {
                for (var z = Mathf.Max(-radius, -x - radius); z <= Mathf.Min(radius, -x + radius); z++)
                {
                    hexInfos.Add(new HexInfo(new CubicCoordinate(x, z), 0, FeatureType.None, 0));
                }
            }
            return hexInfos;
        }
        
        private static Dictionary<CubicCoordinate, HexInfo> CreateHexInfoDictionary(List<HexInfo> hexInfos)
        {
            return hexInfos.ToDictionary(hexInfo => hexInfo.Coordinate);
        }

        private static void AddRandomFaceFeature(HexInfo hexInfo)
        {
            var featureType = hexInfo.Height switch
            {
                2 => FeatureType.Wilderness,
                3 => FeatureType.Wilderness,
                4 => FeatureType.Mountain,
                5 => FeatureType.Mountain,
                _ => FeatureType.None
            };

            var threshold = featureType == FeatureType.Mountain ? 0.75f : 0.4f;
            
            if (Random.value < threshold) return;
            
            hexInfo.FeatureType = featureType;
            hexInfo.FeatureVariation = Random.Range(0, 10);

        }
    }
}