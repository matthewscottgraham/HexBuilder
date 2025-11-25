using App.Events;
using Game.Events;
using Game.Selection;
using UnityEngine;

namespace Game.Hexes
{
    public class CenterTransformOnHover : MonoBehaviour
    {
        private EventBinding<HoverEvent> _hoverEventBinding;

        private void OnEnable()
        {
            _hoverEventBinding = new EventBinding<HoverEvent>(HandleHover);
            EventBus<HoverEvent>.Register(_hoverEventBinding);
        }

        private void OnDisable()
        {
            EventBus<HoverEvent>.Deregister(_hoverEventBinding);
            _hoverEventBinding = null;
        }

        private void HandleHover()
        {
            transform.position = Selector.Hovered.Position;
        }
    }
}