using Game.Hexes;
using Game.Hexes.Features;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class Tool
    {
        public FeatureType FeatureType { get; protected set; }
        public int RadiusIncrement { get; protected set; }
        public Sprite Icon { get; protected set; }
        public bool UseRadius { get; protected set; } = true;
        public SelectionType SelectionType { get; protected set; } = SelectionType.Face;
        protected TilePlacement TilePlacement { get; set; } = TilePlacement.Land;
        
        
        public virtual bool Use(HexObject hex, ToolMode toolMode) 
        { 
            if (!hex) return false;
            if (!VerifyTileHeight(hex)) return false;
            
            var currentFeatureType = hex.Face.FeatureType;

            return toolMode switch
            {
                ToolMode.Add => UseAdditive(hex, currentFeatureType),
                ToolMode.Subtract => UseSubtractive(hex, currentFeatureType),
                ToolMode.Toggle => UseToggle(hex, currentFeatureType),
                _ => false
            }; 
        }

        public virtual bool Use(HexObject[] hexes)
        {
            return false;
        }

        public bool VerifyTileHeight(HexObject hex)
        {
            switch (TilePlacement)
            {
                case TilePlacement.Any:
                    return true;
                case TilePlacement.Land when hex.Height < HexFactory.WaterHeight:
                case TilePlacement.Water when hex.Height >= HexFactory.WaterHeight:
                    return false;
                default:
                    return true;
            }
        }

        private bool UseAdditive(HexObject hex, FeatureType currentFeatureType)
        {
            if (currentFeatureType == FeatureType) return false;
            hex.Face.Add(FeatureType);
            return true;
        }

        private bool UseSubtractive(HexObject hex, FeatureType currentFeatureType)
        {
            if (currentFeatureType == FeatureType.None) return false;
            hex.Face.Set(0, false);
            return true;
        }

        private bool UseToggle(HexObject hex, FeatureType currentFeatureType)
        {
            return currentFeatureType != FeatureType 
                ? UseAdditive(hex, currentFeatureType) 
                : UseSubtractive(hex, currentFeatureType);
        }
    }
}