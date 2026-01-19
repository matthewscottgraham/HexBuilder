using System.Linq;
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
        
        private Transform[] _waterfalls = new Transform[6];
        
        public EdgeFeatures(HexObject owner) : base(owner)
        {
        }

        public Vector3 Position(int edgeIndex) => Owner.Face.Position + HexGrid.GetLocalEdgePosition(edgeIndex);

        public void AddWaterfall(Transform waterfall, int edgeIndex)
        {
            waterfall.SetParent(FeatureParent, false);
            // Add 120 degrees because it is misaligned when it was generated to make the maths easier.
            waterfall.localRotation = Quaternion.Euler(0, edgeIndex * 60 + 120, 0);
            RemoveWaterfall(edgeIndex);
            _waterfalls[edgeIndex] = waterfall;
        }

        public void RemoveWaterfall(int edgeIndex)
        {
            if (_waterfalls[edgeIndex]) Object.Destroy(_waterfalls[edgeIndex].gameObject);
        }
        
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
            FeatureType = HasFeatures.Any(t=> t) ? FeatureType.River : FeatureType.None;
        }

        protected override void Remove(int index)
        {
            base.Remove(index);
            RemoveWaterfall(index);
        }

        protected override void UpdateMesh()
        {
            if (Feature) Object.Destroy(Feature);
            
            if (FeatureType == FeatureType.None || HasFeatures.All(t=> !t)) return;
            
            var featureFactory = ServiceLocator.Instance.Get<FeatureFactory>();
            
            var edgeObject = featureFactory.GetRiverMesh(FeaturesPresent());
            edgeObject.transform.SetParent(FeatureParent, false);
            edgeObject.transform.SetLocalHeight(0.01f);
            Feature = edgeObject;
        }
        
    }
}