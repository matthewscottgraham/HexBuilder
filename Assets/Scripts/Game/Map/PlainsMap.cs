using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Map
{
    public class PlainsMap : IMapStrategy
    {
        public List<HexInfo> GenerateMap()
        {
            var weights = new[] { 3, 2, 2, 2, 1, 0, 1, 2, 2, 2, 3 };
            var hexInfos = new List<HexInfo>();

            var radius = HexGrid.GridRadius;
            
            for (var x = -radius; x <= radius; x++)
            {
                for (var z = Mathf.Max(-radius, -x - radius); z <= Mathf.Min(radius, -x + radius); z++)
                {
                    var height = weights[UnityEngine.Random.Range(0, weights.Length)];
                    hexInfos.Add(new HexInfo(new CubicCoordinate(x, z), height, FeatureType.None, 0));
                }
            }
            return hexInfos;
        }
    }
}