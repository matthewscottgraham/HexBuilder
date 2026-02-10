using System;
using System.Linq;
using App.Services;
using App.Utils;
using Game.Grid;
using UnityEngine;
using Object = UnityEngine.Object;

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
        
        public override CubicCoordinate[] GetCellsClosestToPosition(Vector3 cursorPosition)
        {
            cursorPosition -= Owner.Face.Position;
            cursorPosition.y = 0;
            
            var closestEdgeIndex = -1;
            var closestEdgeDistance = float.MaxValue;
            for (var i = 0; i < 6; i++)
            {
                var edgePos = HexGrid.GetLocalEdgePosition(i);
                var distance = Vector3.Distance(cursorPosition, edgePos);
                if (!(distance < 1f) || !(distance < closestEdgeDistance)) continue;
                closestEdgeIndex = i;
                closestEdgeDistance = distance;
            }
            
            if (closestEdgeIndex < 0) return Array.Empty<CubicCoordinate>();
            var closestNeighbour = HexGrid.GetNeighbourSharingEdge(Owner.Coordinate, closestEdgeIndex);
            return new[] { Owner.Coordinate, closestNeighbour };
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