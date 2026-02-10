using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class AddFarm : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/settlement");
        bool ITool.UseRadius => false;
        
        public bool Use(HexObject hex)
        {
            if (!hex) return false;
            hex.Face.Add(FeatureType.Settlement);
            return true;
        }
    }
}