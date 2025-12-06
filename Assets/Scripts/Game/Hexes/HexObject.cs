using App.Services;
using App.Tweens;
using Game.Features;
using Game.Grid;
using Game.Tools.Paths;
using UnityEngine;

namespace Game.Hexes
{
    public class HexObject : MonoBehaviour
    {
        private const float AnimationDuration = 0.3f;
        private const EaseType AnimationEaseType = EaseType.BounceOut;
        private Feature _feature;
        private readonly GameObject[] _vertexFeatures = new GameObject[6];
        private readonly GameObject[] _vertexBridges = new GameObject[6];
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

        public void Initialize(Coordinate2 coordinate, Transform hexMesh)
        {
            Coordinate = coordinate;
            _hexMesh = hexMesh;
            _hexMesh.SetParent(transform, false);
        }

        public void SetHeight(float height)
        {
            if (height < 0) height = 0;
            Height = height;
            _hexMesh.TweenScale(_hexMesh.transform.localScale, new Vector3(1, height, 1), AnimationDuration).SetEase(AnimationEaseType);
            
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

        public void ToggleVertexFeature(int vertexIndex)
        {
            vertexIndex %= 6;
            if (!_vertexFeatures[vertexIndex])
            {
                AddVertexFeature(vertexIndex);
            }
            else
            {
                RemoveVertexFeature(vertexIndex);
                
            }
            UpdateVertexBridges();
        }

        public Coordinate3? GetVertexCloseToPosition(Vector3 position)
        {
            var closestIndex = -1;
            var closestSquaredDistance = HexGrid.VertexRadius * HexGrid.VertexRadius;
            
            for (var i = 0; i < 6; i++)
            {
                var squaredDistance = (GetVertexPosition(i) - position).sqrMagnitude;
                if (!(squaredDistance <= closestSquaredDistance)) continue;
                closestIndex = i;
                closestSquaredDistance = squaredDistance;
            }
            
            return closestIndex >= 0 ? new Coordinate3(Coordinate, closestIndex) : null;
        }

        public Vector3 GetVertexPosition(int cornerIndex)
        {
            var angleDegrees = 60f * cornerIndex;
            var angleRadians = Mathf.Deg2Rad * angleDegrees;
            var localVertexPosition = new Vector3(Mathf.Sin(angleRadians) * HexGrid.Radius, 0, Mathf.Cos(angleRadians) * HexGrid.Radius);
            return localVertexPosition + transform.position + (Vector3.up * Height);
        }

        private void AddVertexFeature(int vertexIndex)
        {
            if (_vertexFeatures[vertexIndex] != null) return;
            
            var pathController = ServiceLocator.Instance.Get<PathController>();
            var vertexObject = pathController.CreateVertexMesh(GetVertexPosition(vertexIndex));
            vertexObject.transform.SetParent(transform, true);
            _vertexFeatures[vertexIndex] = vertexObject;
        }
        private void RemoveVertexFeature(int vertexIndex)
        {
            if (_vertexFeatures[vertexIndex] == null) return;
            Destroy(_vertexFeatures[vertexIndex]);
        }

        private void AddVertexBridge(int vertexIndex)
        {
            if (_vertexBridges[vertexIndex] != null) return;
            
            var pathController = ServiceLocator.Instance.Get<PathController>();
            var bridgeObject = pathController.CreateBridgeMesh(
                GetVertexPosition(vertexIndex),
                GetVertexPosition((vertexIndex + 1) % 6)
                );
            bridgeObject.transform.SetParent(transform, true);
            _vertexFeatures[vertexIndex] = bridgeObject;
        }

        private void RemoveVertexBridge(int index)
        {
            if (_vertexBridges[index] == null) return;
            Destroy(_vertexBridges[index]);
        }
        private Vector3 GetEdgePosition(int edgeIndex)
        {
            return Vector3.Lerp(GetVertexPosition(edgeIndex), GetVertexPosition((edgeIndex + 1) % 6), 0.5f);
        }

        private Vector3 GetFacePosition()
        {
            return transform.position + new Vector3(0, Height, 0);
        }

        private void UpdateVertexBridges()
        {
            for (var i = 0; i < 6; i++)
            {
                var bridgeRequired = _vertexFeatures[i] != null && _vertexFeatures[(i + 1) % 6] != null;
                
                if (bridgeRequired && _vertexBridges[i] == null) AddVertexBridge(i);
                else RemoveVertexBridge(i);
            }
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