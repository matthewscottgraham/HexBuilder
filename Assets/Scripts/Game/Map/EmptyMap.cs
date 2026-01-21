using System.Collections.Generic;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Map
{
    public class EmptyMap : IMapStrategy
    {
        public List<HexInfo> GenerateMap()
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
    }
}