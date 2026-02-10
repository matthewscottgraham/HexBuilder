using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class LowerTerrain : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/lower");
        public void Use(HexObject hex)
        {
            hex?.SetHeight(hex.Height - 1);
        }
    }
}