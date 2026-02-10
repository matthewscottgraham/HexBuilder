using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class AddMountain : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/mountain");
        bool ITool.UseRadius => false;
        
        public void Use(HexObject hex)
        {
            hex?.Face.Add(FeatureType.Mountain);
        }
    }
}