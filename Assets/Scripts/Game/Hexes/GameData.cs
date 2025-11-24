using System.Collections.Generic;
using Game.Features;

namespace Game.Hexes
{
    public struct GameData
    {
        public GameData(int x, int y)
        {
            Size = new Cell(x, y);
            Map = new List<HexInfo>();
            Features = new Dictionary<int, FeatureType>();
        }

        public Cell Size;
        public List<HexInfo> Map;
        public Dictionary<int, FeatureType> Features;
    }
}