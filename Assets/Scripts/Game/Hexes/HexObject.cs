using App.Tweens;
using UnityEngine;

namespace Game.Hexes
{
    public class HexObject : MonoBehaviour
    {
        private const float AnimationDuration = 0.3f;
        private const EaseType AnimationEaseType = EaseType.BounceOut;
        private CapsuleCollider _collider;
        private GameObject _feature;
        private Transform _hexMesh;

        public Cell Cell { get; private set; }
        public bool HasFeature => _feature != null;
        public float Height { get; private set; } = 1;

        public void Initialize(Cell cell, CapsuleCollider capsuleCollider, Transform hexMesh)
        {
            Cell = cell;
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

        public void AddFeature(GameObject feature)
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
            Destroy(_feature);
        }
    }
}