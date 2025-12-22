using System.Linq;
using App.Services;
using App.Utils;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes.Features
{
    public class VertexFeatures : HexComponent
    {
        protected override string Name => nameof(VertexFeatures);

        public VertexFeatures(HexObject owner) : base(owner)
        {
        }

        public Vector3 Position(int vertexIndex) => HexGrid.GetLocalVertexPosition(vertexIndex) + Owner.Face.Position;

        public override QuarticCoordinate? GetClosestFeatureCoordinate(Vector3 position)
        {
            var closestIndex = -1;
            var closestSquaredDistance = 1f;
            
            for (var i = 0; i < 6; i++)
            {
                var squaredDistance = (Position(i) - position).sqrMagnitude;
                if (!(squaredDistance < closestSquaredDistance)) continue;
                closestIndex = i;
                closestSquaredDistance = squaredDistance;
            }
            
            return closestIndex >= 0 ? new QuarticCoordinate(Owner.Coordinate, closestIndex) : null;
        }
        
        protected override void UpdateFeatureType()
        {
            FeatureType = HasFeatures.Any(t=> t) ? FeatureType.Path : FeatureType.None;
        }

        protected override void AddConnection(int index)
        {
            // if (Connections[index] != null) return;
            //
            // var featureFactory = ServiceLocator.Instance.Get<FeatureFactory>();
            // var bridgeObject = featureFactory.CreateBridgeMesh(
            //     Position(index),
            //     Position((index + 1) % 6)
            // );
            // bridgeObject.transform.SetParent(FeatureParent, true);
            // var localPosition = bridgeObject.transform.localPosition;
            // localPosition.y = 0;
            // bridgeObject.transform.localPosition = localPosition;
            // Features[index] = bridgeObject;
        }

        protected override void UpdateConnections()
        {
            // for (var i = 0; i < 6; i++)
            // {
            //     var bridgeRequired = Features[i] != null && Features[(i + 1) % 6] != null;
            //     
            //     if (bridgeRequired && Connections[i] == null) AddConnection(i);
            //     else RemoveConnection(i);
            // }
        }
        
        protected override void UpdateMesh()
        {
            var featureFactory = ServiceLocator.Instance.Get<FeatureFactory>();
            var edgeObject = featureFactory.GetRiverMesh(FeaturesPresent());
            edgeObject.transform.SetParent(FeatureParent, false);
            edgeObject.transform.SetLocalHeight(0.01f);
            Feature = edgeObject;
        }
    }
}