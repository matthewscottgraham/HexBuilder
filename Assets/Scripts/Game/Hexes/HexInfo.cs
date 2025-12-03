using Game.Features;
using Game.Grid;

namespace Game.Hexes
{
    public class HexInfo
    {
        public readonly Coordinate2 Coordinate;
        public readonly int Height;
        public readonly FeatureType FeatureType;
        public readonly int FeatureVariation;
        public readonly float FeatureRotation;

        public HexInfo(Coordinate2 coordinate, int height, FeatureType featureType, int featureVariation, float featureRotation)
        {
            Coordinate = coordinate;
            Height = height;
            FeatureType = featureType;
            FeatureVariation = featureVariation;
            FeatureRotation = featureRotation;
        }
    }
}