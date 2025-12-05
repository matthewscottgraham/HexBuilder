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
            _hexMesh.TweenScale(_hexMesh.transform.localScale, new Vector3(1, height, 1), AnimationDuration).SetEase(AnimationEaseType);
            _collider.height = Height;
            _collider.center = new Vector3(0, Height / 2f, 0);

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

        private Vector3 GetVertexPosition(int cornerIndex)
        {
            var angleDegrees = 60f * cornerIndex;
            var angleRadians = Mathf.Deg2Rad * angleDegrees;
            var localVertexPosition = new Vector3(Mathf.Sin(angleRadians) * HexGrid.Radius, 0, Mathf.Cos(angleRadians) * HexGrid.Radius);
            return localVertexPosition + transform.position + (Vector3.up * Height);
        }

        private Vector3 GetEdgePosition(int edgeIndex)
        {
            return Vector3.Lerp(GetVertexPosition(edgeIndex), GetVertexPosition((edgeIndex + 1) % 6), 0.5f);
        }

        private Vector3 GetFacePosition()
        {
            return transform.position + new Vector3(0, Height, 0);
        }

        private void OnDrawGizmos()
        {
            var vertices = new Vector3[6];
            for (var i = 0; i < 6; i++)
            {
                vertices[i] = GetVertexPosition(i);
            }
            
            // Face
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetFacePosition(), 1.8f);
            for (var i = 0; i < 6; i++)
            {
                Gizmos.DrawLine(vertices[i], vertices[(i + 1) % 6]);
            }
            
            // Vertices
            Gizmos.color = Color.red;
            for (var i = 0; i < 6; i++)
            {
                Gizmos.DrawWireSphere(vertices[i], 0.6f);
            }
            
            // Edges
            Gizmos.color = Color.blue;
            for (var i = 0; i < 6; i++)
            {
                var pos = Vector3.Lerp(vertices[i], vertices[(i + 1) % 6], 0.5f);
                Gizmos.DrawWireSphere(pos, 0.8f);
            }
        }
    }
}