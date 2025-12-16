using App.Services;
using UnityEngine;

namespace Game.Hexes.Features
{
    public class FaceFeatures : HexComponent
    {
        private FeatureType _currentFeatureType = FeatureType.None;
        protected override string Name => nameof(FaceFeatures);
        
        public FaceFeatures(HexObject owner) : base(owner)
        {
        }

        public Vector3 Position => Owner.transform.position + new Vector3(0, Owner.Height, 0);

        public void Add(FeatureType featureType)
        {
            Remove(0);
            if (_currentFeatureType == featureType) return;
            
            _currentFeatureType = featureType;
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(FeatureType.Wilderness);
            feature.transform.SetParent(FeatureParent, false);
            FeatureParent.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Features[0] = feature;
        }

        public void Add(FeatureType featureType, int variation, float rotation)
        {
            var feature = ServiceLocator.Instance.Get<FeatureFactory>().CreateFeature(featureType, variation);
            feature.transform.SetParent(FeatureParent, false);
            FeatureParent.rotation = Quaternion.Euler(0, rotation, 0);
            Features[0] = feature;
        }
        protected override void UpdateFeatureType()
        {
            FeatureType = Features[0]? _currentFeatureType : FeatureType.None;
        }
    }
}