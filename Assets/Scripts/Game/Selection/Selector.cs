using System;
using System.Collections;
using App.Events;
using App.Input;
using Game.Events;
using UnityEngine;

namespace Game.Selection
{
    public class Selector : MonoBehaviour, IDisposable
    {
        private Camera _camera;
        private Transform _cellHighlighter;
        private EventBinding<InteractEvent> _interactEventBinding;
        private EventBinding<MoveEvent> _moveEventBinding;
        
        public static SelectionContext Hovered { get; private set; }
        
        private void Update()
        {
            if (!InputController.PointerHasMovedThisFrame) return;
            Hover();
        }

        public void Dispose()
        {
            EventBus<MoveEvent>.Deregister(_moveEventBinding);
            EventBus<InteractEvent>.Deregister(_interactEventBinding);
            _moveEventBinding = null;
            _interactEventBinding = null;
        }

        public void Initialize()
        {
            _moveEventBinding = new EventBinding<MoveEvent>(HandleMoveEvent);
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            EventBus<MoveEvent>.Register(_moveEventBinding);
            EventBus<InteractEvent>.Register(_interactEventBinding);

            _camera = Camera.main;
            _cellHighlighter = CreateHighlighter();
        }

        protected virtual Transform CreateHighlighter()
        {
            return null;
        }

        protected virtual global::Game.Selection.SelectionContext GetClampedSelection(Vector3 pos)
        {
            return new global::Game.Selection.SelectionContext();
        }

        private void Hover()
        {
            var ray = _camera.ScreenPointToRay(InputController.PointerPosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            SetHoveredSelection(hit.point);
        }

        private void SetHoveredSelection(Vector3 hoverPosition)
        {
            var originalHover = Hovered;
            var newHover = GetClampedSelection(hoverPosition);
            if (originalHover.Equals(newHover)) return;
            Hovered = newHover;
            transform.position = newHover.Position;
            EventBus<HoverEvent>.Raise(new HoverEvent(newHover));
        }

        private IEnumerator InvokeSelectionEvent()
        {
            yield return new WaitForEndOfFrame();
            EventBus<SelectionEvent>.Raise(new SelectionEvent(Hovered));
        }

        private void HandleInteractEvent(InteractEvent interactEvent)
        {
            StartCoroutine(InvokeSelectionEvent());
        }

        private void HandleMoveEvent(MoveEvent moveEvent)
        {
            var originalHovered = Hovered;
            Vector3 worldPosition = transform.position;
            if (moveEvent.Delta.y != 0)
            {
                var nudge = moveEvent.Delta.y > 0 ? 1 : -1;
                worldPosition += new Vector3( moveEvent.Delta.x * 2 + nudge, 0, transform.position.z + moveEvent.Delta.y * 1.5f);
            }
            else
            {
                worldPosition = new Vector3(moveEvent.Delta.x * 2, 0, moveEvent.Delta.y * 1.5f);
            }

            SetHoveredSelection(worldPosition);
        }
    }
}