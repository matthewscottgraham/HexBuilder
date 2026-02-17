using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class ShiftTerrainTool : Tool
    {
        public override void SetMode(ToolMode mode)
        {
            mode = mode == ToolMode.Toggle ? ToolMode.Add : mode;
            CurrentMode = mode;
        }

        public override ToolMode[] GetModes()
        {
            return ToolModes ??= new[]
            {
                ToolMode.Add,
                ToolMode.Subtract
            };
        }
        
        public ShiftTerrainTool()
        {
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
    }
}