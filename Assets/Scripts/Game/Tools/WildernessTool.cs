using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class WildernessTool : Tool
    {
        public WildernessTool()
        {
            FeatureType = FeatureType.Wilderness;
            Icon = Resources.Load<Sprite>("Sprites/wilderness");
        }
    }
}