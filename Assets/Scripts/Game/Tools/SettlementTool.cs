using Game.Hexes.Features;
using UnityEngine;

namespace Game.Tools
{
    public class SettlementTool : Tool
    {
        public SettlementTool()
        {
            FeatureType = FeatureType.Settlement;
            Icon = Resources.Load<Sprite>("Sprites/settlement");
        }
    }
}