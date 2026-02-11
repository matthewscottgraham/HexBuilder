using App.Events;
using App.Tweens;
using Game.Events;
using Game.Grid;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Hexes
{
    public class HexObject : MonoBehaviour
    {
        private bool _hovered = false;
        private Transform _hexMesh;
        private MeshRenderer _meshRenderer;
        private EventBinding<HoverEvent> _hoverEventBinding;
        
        public static float AnimationDuration => 0.3f;
        public static EaseType AnimationEaseType => EaseType.BounceOut;
        
        public CubicCoordinate Coordinate { get; private set; }
        public int Height { get; private set; } = 1;
        public FaceFeatures Face { get; private set; }
        public EdgeFeatures Edges { get; private set; }
        public VertexFeatures Vertices { get; private set; }
        
        public int Variation { get; private set; }
        
        public void Initialize(CubicCoordinate coordinate, Transform hexMesh)
        {
            Coordinate = coordinate;
            _hexMesh = hexMesh;
            _hexMesh.SetParent(transform, false);

            Face = new FaceFeatures(this);
            Edges = new EdgeFeatures(this);
            Vertices = new VertexFeatures(this);
            
            _meshRenderer = _hexMesh.GetComponent<MeshRenderer>();
            
            _hoverEventBinding = new EventBinding<HoverEvent>(HandleHoverEvent);
            EventBus<HoverEvent>.Register(_hoverEventBinding);
        }

        private void HandleHoverEvent(HoverEvent evt)
        {
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

        public void SetHeight(int height)
        {
            height = Mathf.Clamp(height, 0, HexFactory.MaxHeight);
            Height = height;
            _hexMesh.TweenScale(_hexMesh.transform.localScale, new Vector3(1, height, 1), AnimationDuration)
                .SetEase(AnimationEaseType).SetOnComplete(SetMaterialBasedOnHeight);

            Vertices.SetHeight(height);
            Edges.SetHeight(height);
            Face.SetHeight(height);
        }

        private void SetMaterialBasedOnHeight()
        {
            SetMaterial(HexFactory.GetMaterialForHeight(Height));
        }
        private void SetMaterial(Material material)
        {
            if (!_meshRenderer) return;
            _meshRenderer.sharedMaterial = material;
        }
    }
}