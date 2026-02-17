using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class ShiftTerrainTool : Tool
    {
        public ShiftTerrainTool()
        {
            UseToggleMode = false;
            FeatureType = FeatureType.None;
            Icon = Resources.Load<Sprite>("Sprites/land");
        }

        protected override bool UseAdditive(HexObject hex, FeatureType currentFeatureType)
        {
            return hex.SetHeight(hex.Height + 1);
        }
        
        protected override bool UseSubtractive(HexObject hex, FeatureType currentFeatureType)
        {
            return hex.SetHeight(hex.Height - 1);
        }

        protected override bool UseToggle(HexObject hex, FeatureType currentFeatureType)
        {
            return UseAdditive(hex, currentFeatureType);
        }
    }
}