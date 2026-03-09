using Game.Hexes;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class RaiseTerrainTool : Tool
    {
        public RaiseTerrainTool()
        {
            FeatureType = FeatureType.None;
            Icon = Resources.Load<Sprite>("Sprites/raise");
            PreviewName = "RaiseTerrain";
        }

        public override bool Use(HexObject hex, ToolMode toolMode)
        {
            return hex.SetHeight(hex.Height + 1);
        }
    }
}