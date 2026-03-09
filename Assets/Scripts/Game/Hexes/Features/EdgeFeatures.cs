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
        
        private readonly Transform[] _waterfalls = new Transform[6];
        private readonly FeatureFactory _factory;
        
        public EdgeFeatures(HexObject owner) : base(owner)
        {
            _factory = ServiceLocator.Instance.Get<FeatureFactory>();
        }
        
        public CubicCoordinate[] GetCellsClosestToPosition(Vector3 cursorPosition)
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

        public override void Set(int index, bool hasFeature)
        {
            base.Set(index, hasFeature);
            SetWaterfall(index, hasFeature);
        }

        protected override void UpdateFeatureType()
        {
            FeatureType = HasFeatures.Any(t=> t) ? FeatureType.River : FeatureType.None;
        }

        protected override void Add(int index)
        {
            base.Add(index);
            Owner.Face.SetAll(false);
        }

        protected override void Remove(int index)
        {
            base.Remove(index);
            RemoveWaterfall(index);
            RemoveSharedEdge(index);
        }

        protected override void UpdateMesh()
        {
            if (FeatureType == FeatureType.None || HasFeatures.All(t=> !t)) return;
            var edgeObject = _factory.GetRiverMesh(FeaturesPresent());
            edgeObject?.transform.SetParent(FeatureParent, false);
            edgeObject?.transform.SetLocalHeight(0.01f);
            Feature = edgeObject;
        }

        private void RemoveSilently(int index)
        {
            base.Remove(index);
            RemoveWaterfall(index);
        }

        private void RemoveSharedEdge(int index)
        {
            var neighbour = HexGrid.GetNeighbourSharingEdge(Owner.Coordinate, index);
            if (!HexGrid.InBounds(neighbour)) return;
            var hexController = ServiceLocator.Instance.Get<HexController>();
            var hex = hexController.GetHexObject(neighbour);
            hex.Edges.RemoveSilently(HexGrid.OppositeEdge(index));
        }

        private void SetWaterfall(int edgeIndex, bool hasFeature)
        {
            if (hasFeature)
            {
                AddWaterfall(edgeIndex);
            }
            else
            {
                RemoveWaterfall(edgeIndex);
            }
        }

        private void AddWaterfall(int edgeIndex)
        {
            var neighbour = HexGrid.GetNeighbourSharingEdge(Owner.Coordinate, edgeIndex);
            if (!HexGrid.InBounds(neighbour)) return;
            
            var hexController = ServiceLocator.Instance.Get<HexController>();
            var hex = hexController.GetHexObject(neighbour);
            if (!hex) return;
            if (hex.Height == Owner.Height) return;
            if (_waterfalls[edgeIndex]) RemoveWaterfall(edgeIndex); // The waterfall may need to be resized so remove it

            var waterfall = hexController.WaterfallFactory.CreateWaterFall(Owner, hex);
            waterfall.SetParent(FeatureParent, false);
            // Add 120 degrees because it is misaligned when it was generated to make the maths easier.
            waterfall.localRotation = Quaternion.Euler(0, edgeIndex * 60 + 120, 0);
            _waterfalls[edgeIndex] = waterfall;
        }

        private void RemoveWaterfall(int edgeIndex)
        {
            if (_waterfalls[edgeIndex]) Object.Destroy(_waterfalls[edgeIndex].gameObject);
        }
    }
}