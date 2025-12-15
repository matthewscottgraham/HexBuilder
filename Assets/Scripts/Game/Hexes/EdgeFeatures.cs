using App.Services;
using App.Tweens;
using Game.Features;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class EdgeFeatures : HexComponent
    {
        protected override string Name => nameof(EdgeFeatures);
        
        public EdgeFeatures(HexObject owner) : base(owner)
        {
        }

        public Vector3 Position(int edgeIndex) => Owner.Face.Position + HexGrid.GetLocalEdgePosition(edgeIndex);


        public void SetHeight(int height)
        {
            FeatureParent.TweenLocalPosition(
                    FeatureParent.localPosition, new Vector3(0, height, 0),HexObject.AnimationDuration)
                .SetEase(HexObject.AnimationEaseType);
        }
        
        protected override void Add(int index)
        {
            if (Features[index] != null) return;
            
            var featureFactory = ServiceLocator.Instance.Get<FeatureFactory>();
            var edgeObject = featureFactory.CreateEdgeMesh(Owner.Face.Position, Position(index));
            edgeObject.transform.SetParent(FeatureParent, true);
            Features[index] = edgeObject;
        }
        
    }
}