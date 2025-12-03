using App.Tweens;
using Game.Features;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class HexObject : MonoBehaviour
    {
        private const float AnimationDuration = 0.3f;
        private const EaseType AnimationEaseType = EaseType.BounceOut;
        private CapsuleCollider _collider;
        private Feature _feature;
        private Transform _hexMesh;

        public Coordinate2 Coordinate { get; private set; }
        public  FeatureType FeatureType => _feature?.FeatureType ?? FeatureType.None;

        public int FeatureVariation => _feature ? _feature.Variation : 0;

        public float FeatureRotation
        {
            get
            {
                if (_feature) return transform.localRotation.eulerAngles.y;
                return 0;
            }
        }

        
        public float Height { get; private set; } = 1;

        public void Initialize(Coordinate2 coordinate, CapsuleCollider capsuleCollider, Transform hexMesh)
        {
            Coordinate = coordinate;
            _collider = capsuleCollider;
            _hexMesh = hexMesh;
            _hexMesh.SetParent(transform, false);
        }

        public void SetHeight(float height)
        {
            if (height < 0) height = 0;
            Height = height;
            _hexMesh.TweenScale(Vector3.zero, new Vector3(1, height, 1), AnimationDuration).SetEase(AnimationEaseType);
            _collider.height = Height;

            if (_feature == null) return;
            _feature.transform
                .TweenLocalPosition(_feature.transform.localPosition, new Vector3(0, Height, 0), AnimationDuration)
                .SetEase(AnimationEaseType);
        }

        public void AddFeature(Feature feature)
        {
            RemoveFeature();
            _feature = feature;

            if (_feature == null) return;
            _feature.transform.SetParent(transform, false);
            _feature.transform.localPosition = new Vector3(0, Height, 0);
        }

        public void RemoveFeature()
        {
            if (_feature == null) return;
            Destroy(_feature.gameObject);
        }
    }
}