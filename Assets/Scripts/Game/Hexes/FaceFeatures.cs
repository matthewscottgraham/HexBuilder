using App.Tweens;
using Game.Features;
using UnityEngine;

namespace Game.Hexes
{
    public class FaceFeatures : HexComponent
    {
        private Feature _feature;
        
        public  FeatureType FeatureType => _feature?.FeatureType ?? FeatureType.None;
        public int FeatureVariation => _feature ? _feature.Variation : 0;
        public float FeatureRotation
        {
            get
            {
                if (_feature) return _feature.transform.localRotation.eulerAngles.y;
                return 0;
            }
        }
        
        protected override string Name => nameof(FaceFeatures);

        public FaceFeatures(HexObject owner) : base(owner)
        {
        }

        public Vector3 Position => Owner.transform.position + new Vector3(0, Owner.Height, 0);

        public void SetHeight(int height)
        {
            if (!_feature) return;
            _feature.transform.TweenLocalPosition(
                    _feature.transform.localPosition, new Vector3(0, height, 0),HexObject.AnimationDuration)
                .SetEase(HexObject.AnimationEaseType);
        }

        public void AddFeature(Feature feature)
        {
            Remove(0);
            _feature = feature;

            if (_feature == null) return;
            _feature.transform.SetParent(Owner.transform, false);
            _feature.transform.localPosition = new Vector3(0, Owner.Height, 0);
        }
    }
}