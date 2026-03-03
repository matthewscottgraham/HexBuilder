using App.Events;
using App.Services;
using App.Tweens;
using Game.Events;
using Game.Grid;
using Game.Hexes.Features;
using Game.Selection;
using UnityEngine;

namespace Game.Hexes
{
    public class HexObject : MonoBehaviour
    {
        private HexController _hexController;
        private bool _hovered;
        private Transform _hexMesh;
        private Transform _hexTop;
        private MeshRenderer[] _meshRenderers;
        private EventBinding<HoverEvent> _hoverEventBinding;
        
        public static float AnimationDuration => 0.3f;
        public static EaseType AnimationEaseType => EaseType.BounceOut;
        
        public CubicCoordinate Coordinate { get; private set; }
        public int Height { get; private set; } = 1;
        public FaceFeatures Face { get; private set; }
        public EdgeFeatures Edges { get; private set; }
        public VertexFeatures Vertices { get; private set; }
        
        public int Variation { get; private set; }
        
        public void Initialize(CubicCoordinate coordinate, Transform hexMesh, Transform hexTop)
        {
            _hexController = ServiceLocator.Instance.Get<HexController>();
            Coordinate = coordinate;
            _hexMesh = hexMesh;
            _hexMesh.SetParent(transform, false);
            
            _hexTop = hexTop;
            _hexTop.SetParent(transform, false);
            
            Face = new FaceFeatures(this);
            Edges = new EdgeFeatures(this);
            Vertices = new VertexFeatures(this);

            _meshRenderers = new[]
            {
                _hexMesh.GetComponentInChildren<MeshRenderer>(),
                _hexTop.GetComponentInChildren<MeshRenderer>()
            };
            
            _hoverEventBinding = new EventBinding<HoverEvent>(HandleHoverEvent);
            EventBus<HoverEvent>.Register(_hoverEventBinding);
        }

        private void HandleHoverEvent(HoverEvent evt)
        {
            if (evt.HoverSelection.SelectionType == SelectionType.None)
            {
                DeHover();
                return;
            }
            
            foreach (var coordinate in evt.HoverSelection.Coordinates)
            {
                if (!coordinate.Equals(Coordinate)) continue;
                Hover();
                return;
            }
            DeHover();
        }
        
        private void Hover()
        {
            _hovered = true;
            SetMaterial(HexFactory.HighlightMaterial);
        }

        private void DeHover()
        {
            if (!_hovered) return;
            _hovered = false;
            SetMaterialBasedOnHeight();
        }

        public bool SetHeight(int height)
        {
            height = Mathf.Clamp(height, 0, HexFactory.MaxHeight);
            if (Height == height) return false;
            Height = height;
            
            _hexMesh.TweenScale(_hexMesh.transform.localScale, new Vector3(1, height - 0.1f, 1), AnimationDuration)
                .SetEase(AnimationEaseType).SetOnComplete(SetMaterialBasedOnHeight);
            
            _hexTop.TweenLocalPosition(_hexTop.transform.localPosition, new Vector3(0, height, 0), AnimationDuration)
                .SetEase(AnimationEaseType);

            Vertices.SetHeight(height);
            Edges.SetHeight(height);
            Face.SetHeight(height);
            
            return true;
        }

        public bool SetHeightImmediately(int height)
        {
            Height = Mathf.Clamp(height, 0, HexFactory.MaxHeight);

            _hexMesh.localScale = new Vector3(1, Height - 0.1f, 1);
            _hexTop.localPosition = new Vector3(0, Height, 0);
            
            Vertices.SetHeightImmediately(Height);
            Edges.SetHeightImmediately(Height);
            Face.SetHeightImmediately(Height);

            SetMaterialBasedOnHeight();
            
            return true;
        }

        private void SetMaterialBasedOnHeight()
        {
            SetMaterial(HexFactory.GetMaterialForHeight(Height));
        }
        private void SetMaterial(Material material)
        {
            if (_meshRenderers == null || _meshRenderers.Length == 0) return;
            foreach (var meshRenderer in _meshRenderers)
            {
                if (meshRenderer) meshRenderer.sharedMaterial = material;
            }
            
        }
    }
}