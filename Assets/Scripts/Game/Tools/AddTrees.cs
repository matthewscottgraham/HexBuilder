using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class AddTrees : ITool
    {
        Sprite ITool.Icon => Resources.Load<Sprite>("Sprites/wilderness");
        bool ITool.UseRadius => false;
        
        public void Use(HexObject hex)
        {
            hex?.Face.Add(FeatureType.Wilderness);
        }
    }
}