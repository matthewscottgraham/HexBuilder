using Game.Hexes;
using Game.Hexes.Features;
using Game.Selection;
using UnityEngine;

namespace Game.Tools
{
    public class Tool
    {
        protected FeatureType FeatureType;
        
        public int RadiusIncrement { get; protected set; } = 0;
        public Sprite Icon { get; protected set; }
        public bool UseMode { get; protected set; } = true;
        public bool UseToggleMode { get; protected set; } = true;
        public bool UseRadius { get; protected set; } = true;
        public SelectionType SelectionType { get; protected set; } = SelectionType.Face;
        public virtual bool Use(HexObject hex, ToolMode mode = ToolMode.Add) 
        { 
            if (!hex) return false;
            var currentFeatureType = hex.Face.FeatureType;

            return mode switch
            {
                ToolMode.Add => UseAdditive(hex, currentFeatureType),
                ToolMode.Subtract => UseSubtractive(hex, currentFeatureType),
                ToolMode.Toggle => UseToggle(hex, currentFeatureType),
                _ => false
            }; 
        }
        public virtual bool Use(HexObject[] hexes, ToolMode mode = ToolMode.Add){ return false; }
        
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
        
        protected virtual bool UseToggle(HexObject hex, FeatureType currentFeatureType)
        {
            return currentFeatureType != FeatureType 
                ? UseAdditive(hex, currentFeatureType) 
                : UseSubtractive(hex, currentFeatureType);
        }
    }
}