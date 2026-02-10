using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/lower");
        public bool Use(HexObject hex)
        {
            if (!hex) return false;
            hex.SetHeight(hex.Height - 1);
            return true;
        }
    }
}