using System.Linq;
using App.Services;
using App.Tweens;
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
            FeatureVariation = _factory.GetRandomPrefabIndex(FeatureType);
            var feature = _factory.CreateFeature(featureType, FeatureVariation);
            feature.transform.SetParent(FeatureParent, false);
            Feature = feature;
            HasFeatures[0] = true;

            if (Owner.Edges.AnyFeaturesPresent)
            {
                Owner.Edges.SetAll(false);
            }
            
            SetHeight(Owner.Height);
        }

        public void Add(FeatureType featureType, int variation, int rotation)
        {
            FeatureType = featureType;
            FeatureVariation = _factory.GetRandomPrefabIndex(FeatureType);
            var feature = _factory.CreateFeature(featureType, variation);
            feature.transform.SetParent(FeatureParent, false);
            Feature = feature;
            HasFeatures[0] = true;
            FeatureParent.localRotation = Quaternion.Euler(0, rotation, 0);
            SetHeightImmediately(Owner.Height);
        }
        protected override void UpdateFeatureType()
        {
            FeatureType = HasFeatures.Any(t=> t) ? FeatureType : FeatureType.None;
        }

        public override void SetHeight(int height)
        {
            if (height < HexFactory.WaterHeight && FeatureType != FeatureType.Water) Remove(0);
            if (height >= HexFactory.WaterHeight && FeatureType == FeatureType.Water) Remove(0);

            if (FeatureType == FeatureType.Water)
            {
                if (Feature.GetComponent<Floater>())
                    height = HexFactory.WaterHeight;
            }
            
            base.SetHeight(height);
        }

        public override void SetHeightImmediately(int height)
        {
            if (height < HexFactory.WaterHeight && FeatureType != FeatureType.Water) Remove(0);
            if (height >= HexFactory.WaterHeight && FeatureType == FeatureType.Water) Remove(0);

            if (FeatureType == FeatureType.Water)
            {
                if (Feature.GetComponent<Floater>())
                    height = HexFactory.WaterHeight;
            }
            
            base.SetHeightImmediately(height);
        }
    }
}