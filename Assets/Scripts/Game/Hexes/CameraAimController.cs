using App.Events;
using Game.Events;
using Game.Selection;
using UnityEngine;

namespace Game.Hexes
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