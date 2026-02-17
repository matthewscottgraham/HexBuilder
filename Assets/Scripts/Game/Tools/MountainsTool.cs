using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class MountainsTool : Tool
    {
        public MountainsTool()
        {
            FeatureType = FeatureType.Mountain;
            Icon = Resources.Load<Sprite>("Sprites/mountain");
        }
    }
}