using System;
using System.Collections.Generic;
using Game.Hexes.Features;

namespace Game.Hexes
{
    [Serializable]
    public class GameData
    {
        public GameData(int gridRadius, List<HexInfo> hexInfos)
        {
            hexInfos ??= new List<HexInfo>();
            this.gridRadius = gridRadius;
            Map = hexInfos;
            Features = new Dictionary<int, FeatureType>();
        }

        public int gridRadius;
        public List<HexInfo> Map;
        public Dictionary<int, FeatureType> Features;
    }
}