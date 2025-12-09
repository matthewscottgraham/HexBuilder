using App.Events;
using UnityEngine;

namespace Game.Cameras
{
    public class CameraAimController : MonoBehaviour
    {
        private readonly float _sensitivity = 0.5f;
        private EventBinding<DragEvent> _dragEvent;

        private void OnEnable()
        {
            _dragEvent = new EventBinding<DragEvent>(HandleDrag);
            EventBus<DragEvent>.Register(_dragEvent);
        }

        private void OnDisable()
        {
            EventBus<DragEvent>.Deregister(_dragEvent);
            _dragEvent = null;
        }

        private void HandleDrag(DragEvent ev)
        {
            transform.position += new Vector3(ev.Delta.x, 0, ev.Delta.y) * _sensitivity;
        }
    }
}