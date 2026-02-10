using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class RaiseTerrain : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/raise");
        public bool CreateHexesAsNeeded => true;
        
        public bool Use(HexObject hex)
        {
            if (!hex) return false;
            hex.SetHeight(hex.Height + 1);
            return true;
        }
    }
}