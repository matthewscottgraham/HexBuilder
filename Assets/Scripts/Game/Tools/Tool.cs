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

        protected ToolMode[] ToolModes;
        
        public virtual bool Use(HexObject hex, ToolMode toolMode) 
        { 
            if (!hex) return false;
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
        
        protected virtual bool UseAdditive(HexObject hex, FeatureType currentFeatureType)
        {
            if (currentFeatureType == FeatureType) return false;
            hex.Face.Add(FeatureType);
            return true;
        }
        
        protected virtual bool UseSubtractive(HexObject hex, FeatureType currentFeatureType)
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