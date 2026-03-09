using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class LowerTerrainTool : Tool
    {
        public LowerTerrainTool()
        {
            FeatureType = FeatureType.None;
            Icon = Resources.Load<Sprite>("Sprites/lower");
            PreviewName = "LowerTerrain";
        }
        
        public override bool Use(HexObject hex, ToolMode toolMode)
        {
            return hex.SetHeight(hex.Height - 1);
        }
    }
}