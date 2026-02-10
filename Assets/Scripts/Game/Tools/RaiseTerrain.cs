using Game.Hexes;
using UnityEngine;

namespace Game.Tools
{
    public class RaiseTerrain : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/raise");
        public bool CreateHexesAsNeeded => true;
        
        public void Use(HexObject hex)
        {
            hex?.SetHeight(hex.Height + 1);
        }
    }
}