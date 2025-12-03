using System.Collections.Generic;
using Game.Features;
using Game.Grid;

namespace Game.Hexes
{
    public struct GameData
    {
        public GameData(int x, int y)
        {
            Size = new Coordinate2(x, y);
            Map = new List<HexInfo>();
            Features = new Dictionary<int, FeatureType>();
        }

        public Coordinate2 Size;
        public List<HexInfo> Map;
        public Dictionary<int, FeatureType> Features;
    }
}