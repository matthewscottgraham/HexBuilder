using App.Events;
using UnityEngine;

namespace Game.Cameras
{
    public class CameraAimController : MonoBehaviour
    {
        private readonly Vector2 _dragSensitivity = new(60f, 90f);
        private readonly Vector2 _moveSensitivity = new(10f, 30f);
        private readonly Vector2 _zoomSensitivity = new(30f, 80f);
        private readonly Vector2 _zoomMinMax = new(-7, 15);
        
        private EventBinding<DragEvent> _dragEventBinding;
        private EventBinding<MoveEvent> _moveEventBinding;
        private EventBinding<ZoomEvent> _zoomEventBinding;

        private void OnEnable()
        {
            _dragEventBinding = new EventBinding<DragEvent>(HandleDrag);
            EventBus<DragEvent>.Register(_dragEventBinding);
            
            _moveEventBinding = new EventBinding<MoveEvent>(HandleMove);
            EventBus<MoveEvent>.Register(_moveEventBinding);
            
            _zoomEventBinding = new EventBinding<ZoomEvent>(HandleZoom);
            EventBus<ZoomEvent>.Register(_zoomEventBinding);
        }

        private void OnDisable()
        {
            EventBus<DragEvent>.Deregister(_dragEventBinding);
            _dragEventBinding = null;
            
            EventBus<MoveEvent>.Deregister(_moveEventBinding);
            _moveEventBinding = null;
            
            EventBus<ZoomEvent>.Deregister(_zoomEventBinding);
            _zoomEventBinding = null;
        }

        private void HandleDrag(DragEvent ev)
        {
            transform.position += new Vector3(ev.Delta.x, 0, ev.Delta.y) * GetSensitivity(_dragSensitivity) * Time.deltaTime;
        }

        private void HandleMove(MoveEvent ev)
        {
            transform.position += new Vector3(ev.Delta.x, 0, ev.Delta.y) * GetSensitivity(_moveSensitivity) * Time.deltaTime;
        }

        private void HandleZoom(ZoomEvent ev)
        {
            var newPosition = transform.position + new Vector3(0, ev.Delta, 0) * GetSensitivity(_zoomSensitivity) * Time.deltaTime;
            newPosition.y = Mathf.Clamp(newPosition.y, _zoomMinMax.x, _zoomMinMax.y);
            transform.position = newPosition;
        }

        private float GetSensitivity(Vector2 range)
        {
            var height = Mathf.InverseLerp(_zoomMinMax.x, _zoomMinMax.y, transform.position.y);
            return Mathf.Lerp(range.x, range.y, height);
        }
    }
}