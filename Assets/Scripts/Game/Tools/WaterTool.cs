using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class WaterTool : Tool
    {
        public WaterTool()
        {
            FeatureType = FeatureType.Water;
            Icon = Resources.Load<Sprite>("Sprites/water");
            TilePlacement = TilePlacement.Water;
        }
    }
}