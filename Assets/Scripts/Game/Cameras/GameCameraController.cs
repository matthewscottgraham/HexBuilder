using System.Linq;
using App.Events;
using Game.Grid;
using Game.Selection;
using UnityEngine;

namespace Game.Cameras
{
    public class GameCameraController : MonoBehaviour
    {
        private readonly Vector2 _dragSensitivity = new(60f, 90f);
        private readonly Vector2 _rotateSensitivity = new(150f, 250f);
        private readonly Vector2 _zoomSensitivity = new(30f, 80f);
        private readonly Vector2 _zoomMinMax = new(-7, 15);

        private Vector3? _focusPosition;
        private float _focusTimer;
        private float _currentYaw;
        private float _currentZoom;
        
        private EventBinding<DragEvent> _dragEventBinding;
        private EventBinding<ZoomEvent> _zoomEventBinding;
        private EventBinding<FocusEvent> _focusEventBinding;
        private EventBinding<RotateEvent> _rotateEventBinding;

        private void OnEnable()
        {
            _dragEventBinding = new EventBinding<DragEvent>(HandleDrag);
            EventBus<DragEvent>.Register(_dragEventBinding);
            
            _zoomEventBinding = new EventBinding<ZoomEvent>(HandleZoom);
            EventBus<ZoomEvent>.Register(_zoomEventBinding);

            _focusEventBinding = new EventBinding<FocusEvent>(HandleFocus);
            EventBus<FocusEvent>.Register(_focusEventBinding);

            _rotateEventBinding = new EventBinding<RotateEvent>(HandleRotate);
            EventBus<RotateEvent>.Register(_rotateEventBinding);
        }

        private void OnDisable()
        {
            EventBus<DragEvent>.Deregister(_dragEventBinding);
            _dragEventBinding = null;
            
            EventBus<ZoomEvent>.Deregister(_zoomEventBinding);
            _zoomEventBinding = null;
            
            EventBus<FocusEvent>.Deregister(_focusEventBinding);
            _focusEventBinding = null;
            
            EventBus<RotateEvent>.Deregister(_rotateEventBinding);
            _rotateEventBinding = null;
        }

        private void Update()
        {
            if (!_focusPosition.HasValue) return;
            transform.position = Vector3.Slerp(transform.position, _focusPosition.Value, _focusTimer);
            _focusTimer += Time.deltaTime;
            if (_focusTimer > 2) _focusPosition = null;
        }

        private void HandleDrag(DragEvent ev)
        {
            _focusPosition = null;
            var right = transform.right;
            var forward = Vector3.Cross(right, Vector3.up);

            transform.localPosition += (right * ev.Delta.x + forward * ev.Delta.y)
                                       * GetSensitivity(_dragSensitivity) * Time.deltaTime;
        }

        private void HandleZoom(ZoomEvent ev)
        {
            var newPosition = transform.position + new Vector3(0, ev.Delta, 0) * GetSensitivity(_zoomSensitivity) * Time.deltaTime;
            newPosition.y = Mathf.Clamp(newPosition.y, _zoomMinMax.x, _zoomMinMax.y);
            transform.localPosition = newPosition;
        }

        private void HandleRotate(RotateEvent ev)
        {
            _currentYaw -= ev.Delta.x * GetSensitivity(_rotateSensitivity) * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0, _currentYaw, 0);
        }

        private void HandleFocus(FocusEvent ev)
        {
            _focusTimer = 0;
            if (Selector.Hovered.Coordinates.Count == 0) return;
            var hoveredCoordinate = Selector.Hovered.Coordinates.FirstOrDefault();
            var focusPosition = HexGrid.GetWorldPosition(hoveredCoordinate);
            _focusPosition = focusPosition;
        }

        private float GetSensitivity(Vector2 range)
        {
            var height = Mathf.InverseLerp(_zoomMinMax.x, _zoomMinMax.y, transform.position.y);
            return Mathf.Lerp(range.x, range.y, height);
        }
    }
}