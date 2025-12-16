using App.Services;
using App.Utils;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes.Features
{
    public class EdgeFeatures : HexComponent
    {
        public override FeatureType FeatureType { get; protected set; }
        protected override string Name => nameof(EdgeFeatures);
        
        public EdgeFeatures(HexObject owner) : base(owner)
        {
        }

        public Vector3 Position(int edgeIndex) => Owner.Face.Position + HexGrid.GetLocalEdgePosition(edgeIndex);
        
        public override QuarticCoordinate? GetClosestFeatureCoordinate(Vector3 position)
        {
            var closestVertex = Owner.Vertices.GetClosestFeatureCoordinate(position);
            if (!closestVertex.HasValue) return null;
            
            var cubicCoordinate = closestVertex.Value.CubicCoordinate;
            var componentIndex = closestVertex.Value.W;
            
            var prev = Owner.Vertices.Position((componentIndex + 5) % 6);
            var current = Owner.Vertices.Position(componentIndex);
            var next = Owner.Vertices.Position((componentIndex + 1) % 6);

            return Vector3.Distance(prev, current) < Vector3.Distance(current, next) 
                ? new QuarticCoordinate(cubicCoordinate, (componentIndex + 5) % 6) 
                : new QuarticCoordinate(cubicCoordinate, componentIndex);
        }
        
        protected override void UpdateFeatureType()
        {
            FeatureType = Features[0]? FeatureType.River : FeatureType.None;
        }
        
        protected override void Add(int index)
        {
            if (Features[index] != null) return;
            
            var featureFactory = ServiceLocator.Instance.Get<FeatureFactory>();
            var edgeObject = featureFactory.CreateEdgeMesh(Owner.Face.Position, Position(index));
            edgeObject.transform.SetParent(FeatureParent, true);
            edgeObject.transform.SetLocalHeight(0);
            Features[index] = edgeObject;
        }
        
    }
}