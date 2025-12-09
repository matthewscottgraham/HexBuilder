using System.Collections.Generic;
using Game.Features;

namespace Game.Hexes
{
    public readonly struct GameData
    {
        public GameData(int gridRadius, List<HexInfo> hexInfos)
        {
            GridRadius = gridRadius;
            Map = hexInfos;
            Features = new Dictionary<int, FeatureType>();
        }

        public readonly int GridRadius;
        public readonly List<HexInfo> Map;
        public readonly Dictionary<int, FeatureType> Features;
    }
}