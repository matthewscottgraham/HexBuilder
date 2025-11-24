using Game.Features;

namespace Game.Hexes
{
    public class HexInfo
    {
        public readonly Cell Cell;
        public readonly int Height;
        public readonly FeatureType FeatureType;
        public readonly int FeatureVariation;
        public readonly float FeatureRotation;

        public HexInfo(Cell cell, int height, FeatureType featureType, int featureVariation, float featureRotation)
        {
            Cell = cell;
            Height = height;
            FeatureType = featureType;
            FeatureVariation = featureVariation;
            FeatureRotation = featureRotation;
        }
    }
}