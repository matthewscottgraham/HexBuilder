using System;
using System.Linq;
using App.Services;
using App.Utils;
using Game.Grid;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Hexes.Features
{
    public class VertexFeatures : HexComponent
    {
        private readonly FeatureFactory _factory;
        
        protected override string Name => nameof(VertexFeatures);

        public VertexFeatures(HexObject owner) : base(owner)
        {
            _factory = ServiceLocator.Instance.Get<FeatureFactory>();
        }

        public Vector3 Position(int vertexIndex) => HexGrid.GetLocalVertexPosition(vertexIndex) + Owner.Face.Position;

        public CubicCoordinate[] GetCellsClosestToPosition(Vector3 cursorPosition)
        {
            cursorPosition -= Owner.Face.Position;
            cursorPosition.y = 0;
            
            var closestVertexIndex = -1;
            var closestVertexDistance = float.MaxValue;
            for (var i = 0; i < 6; i++)
            {
                var edgePos = HexGrid.GetLocalEdgePosition(i);
                var distance = Vector3.Distance(cursorPosition, edgePos);
                if (!(distance < 1f) || !(distance < closestVertexDistance)) continue;
                closestVertexIndex = i;
                closestVertexDistance = distance;
            }
            
            if (closestVertexIndex < 0) return Array.Empty<CubicCoordinate>();
            var closestNeighbour = HexGrid.GetNeighboursSharingVertex(Owner.Coordinate, closestVertexIndex);
            return new[] { Owner.Coordinate, closestNeighbour.A, closestNeighbour.B };
        }
        
        protected override void UpdateFeatureType()
        {
            FeatureType = HasFeatures.Any(t=> t) ? FeatureType.Path : FeatureType.None;
        }
        
        protected override void UpdateMesh()
        {
            if (FeatureType == FeatureType.None || HasFeatures.All(t=> !t)) return;
            var vertexObject = _factory.GetPathMesh(FeaturesPresent());
            vertexObject?.transform.SetParent(FeatureParent, false);
            vertexObject?.transform.SetLocalHeight(0.01f);
            Feature = vertexObject;
        }
    }
}