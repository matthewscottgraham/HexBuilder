using Game.Features;
using Game.Grid;

namespace Game.Hexes
{
    public class HexInfo
    {
        public readonly CubicCoordinate Coordinate;
        public readonly int Height;
        public readonly FeatureType FeatureType;
        public readonly int FeatureVariation;
        public readonly float FeatureRotation;

        public HexInfo(HexObject hexObject)
        {
            Coordinate = hexObject.Coordinate;
            Height = (int)hexObject.Height;
            FeatureType = hexObject.FeatureType;
            FeatureVariation = hexObject.FeatureVariation;
            FeatureRotation = hexObject.FeatureRotation;
        }

        public HexInfo(CubicCoordinate coordinate, int height, FeatureType featureType, int featureVariation, float featureRotation)
        {
            Coordinate = coordinate;
            Height = height;
            FeatureType = featureType;
            FeatureVariation = featureVariation;
            FeatureRotation = featureRotation;
        }
    }
}