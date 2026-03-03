using System.Linq;
using App.Services;
using UnityEngine;

namespace Game.Hexes.Features
{
    public class FaceFeatures : HexComponent
    {
        private readonly FeatureFactory _factory;
        protected override string Name => nameof(FaceFeatures);
        
        public FaceFeatures(HexObject owner) : base(owner)
        {
            _factory = ServiceLocator.Instance.Get<FeatureFactory>();
        }

        public Vector3 Position => Owner.transform.position + new Vector3(0, Owner.Height, 0);

        public void Add(FeatureType featureType)
        {
            Remove(0);
            FeatureType = featureType;
            var feature = _factory.CreateFeature(featureType);
            feature.transform.SetParent(FeatureParent, false);
            Feature = feature;
            HasFeatures[0] = true;
        }

        public void Add(FeatureType featureType, int variation)
        {
            var feature = _factory.CreateFeature(featureType, variation);
            feature.transform.SetParent(FeatureParent, false);
            Feature = feature;
            HasFeatures[0] = true;
        }
        protected override void UpdateFeatureType()
        {
            FeatureType = HasFeatures.Any(t=> t) ? FeatureType : FeatureType.None;
        }

        public override void SetHeight(int height)
        {
            if (height < HexFactory.WaterHeight && FeatureType != FeatureType.Water) Remove(0);
            if (height >= HexFactory.WaterHeight && FeatureType == FeatureType.Water) Remove(0);
            base.SetHeight(height);
        }
    }
}