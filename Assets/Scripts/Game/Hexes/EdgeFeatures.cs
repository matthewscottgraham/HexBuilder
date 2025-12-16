using App.Services;
using App.Utils;
using Game.Grid;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Hexes
{
    public class EdgeFeatures : HexComponent
    {
        public override FeatureType FeatureType { get; protected set; }
        protected override string Name => nameof(EdgeFeatures);
        
        public EdgeFeatures(HexObject owner) : base(owner)
        {
        }

        public Vector3 Position(int edgeIndex) => Owner.Face.Position + HexGrid.GetLocalEdgePosition(edgeIndex);

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