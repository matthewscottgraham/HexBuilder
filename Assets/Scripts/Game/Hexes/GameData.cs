using App.SaveData;
using System.Collections.Generic;

namespace Game.Hexes
{
    public struct GameData
    {
        public GameData(int x, int y)
        {
            Size = new Cell(x, y);
            Map = new List<CellEntry>();
        }
        
        public Cell Size;
        public List<CellEntry> Map;
    }
}