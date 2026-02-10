using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LevelTerrain : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/level");
        public bool CreateHexesAsNeeded => true;
        public int Level { get; set; }

        public bool Use(HexObject hex)
        {
            if (!hex) return false;
            hex.SetHeight(Level);
            return true;
        }
    }
}