using App.SaveData;
using System.Collections.Generic;

namespace Game.Hexes
{
    public struct GameData
    {
        public GameData(int x, int y)
        {
            Size.x = x;
            Size.y = y;
            Map = new Dictionary<Coordinate, int>();
        }
        
        public Coordinate Size;
        public Dictionary<Coordinate, int> Map;
    }
}