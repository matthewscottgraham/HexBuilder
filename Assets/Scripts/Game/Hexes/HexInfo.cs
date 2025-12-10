using Game.Features;
using Game.Grid;
using Newtonsoft.Json;

namespace Game.Hexes
{
    public class HexInfo
    {
        public readonly CubicCoordinate Coordinate;
        public readonly int Height;
        public readonly FeatureType FeatureType;
        public readonly int FeatureVariation;
        public readonly float FeatureRotation;
        public readonly bool[] VertexFeatures;

        public HexInfo(HexObject hexObject)
        {
            Coordinate = hexObject.Coordinate;
            Height = hexObject.Height;
            FeatureType = hexObject.FeatureType;
            FeatureVariation = hexObject.FeatureVariation;
            FeatureRotation = hexObject.FeatureRotation;
            VertexFeatures = hexObject.VertexFeaturesPresent();
        }

        [JsonConstructor]
        public HexInfo(CubicCoordinate coordinate, int height, FeatureType featureType, int featureVariation,
            float featureRotation, bool[] vertexFeatures = null)
        {
            Coordinate = coordinate;
            Height = height;
            FeatureType = featureType;
            FeatureVariation = featureVariation;
            FeatureRotation = featureRotation;

            vertexFeatures ??= new bool[6];
            VertexFeatures = vertexFeatures;
        }
    }
}