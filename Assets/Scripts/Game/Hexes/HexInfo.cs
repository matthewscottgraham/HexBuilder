using Game.Grid;
using Game.Hexes.Features;
using Newtonsoft.Json;

namespace Game.Hexes
{
    public class HexInfo
    {
        public readonly CubicCoordinate Coordinate;
        public int Height;
        public FeatureType FeatureType;
        public int FeatureVariation;
        public int FeatureRotation;
        public readonly bool[] VertexFeatures;
        public readonly bool[] EdgeFeatures;

        public HexInfo(HexObject hexObject)
        {
            Coordinate = hexObject.Coordinate;
            Height = hexObject.Height;
            FeatureType = hexObject.Face.FeatureType;
            FeatureVariation = hexObject.Face.FeatureVariation;
            FeatureRotation = hexObject.Face.FeatureRotation;
            VertexFeatures = hexObject.Vertices.FeaturesPresent();
            EdgeFeatures = hexObject.Edges.FeaturesPresent();
        }

        [JsonConstructor]
        public HexInfo(CubicCoordinate coordinate, int height, 
            FeatureType featureType, int featureVariation, int featureRotation,
            bool[] edgeFeatures = null, bool[] vertexFeatures = null)
        {
            Coordinate = coordinate;
            Height = height;
            
            FeatureType = featureType;
            FeatureVariation = featureVariation;
            FeatureRotation = featureRotation;

            edgeFeatures ??= new bool[6];
            EdgeFeatures = edgeFeatures;
            
            vertexFeatures ??= new bool[6];
            VertexFeatures = vertexFeatures;
        }
    }
}