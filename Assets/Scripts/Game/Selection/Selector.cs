using System;
using System.Collections;
using App.Events;
using App.Input;
using Game.Events;
using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public class Selector : MonoBehaviour, IDisposable
    {
        protected HexGrid HexGrid;
        protected HexController HexController;
        protected Transform CellHighlighter;

        private Camera _camera;
        private EventBinding<InteractEvent> _interactEventBinding;
        private EventBinding<MoveEvent> _moveEventBinding;
        private bool _isActive;
        
        public static SelectionContext Hovered { get; private set; }
        public virtual SelectionType SelectionType => SelectionType.None;
        
        public void Activate(bool isActive)
        {
            _isActive = isActive;
        }
        
        private void Update()
        {
            if (!_isActive) return;
            if (!InputController.PointerHasMovedThisFrame) return;
            Hover();
        }

        public void Initialize(HexGrid hexGrid, HexController hexController)
        {
            HexGrid = hexGrid;
            HexController = hexController;
            
            _moveEventBinding = new EventBinding<MoveEvent>(HandleMoveEvent);
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            EventBus<MoveEvent>.Register(_moveEventBinding);
            EventBus<InteractEvent>.Register(_interactEventBinding);

            _camera = Camera.main;
            CellHighlighter = CreateHighlighter();
        }

        public void Dispose()
        {
            EventBus<MoveEvent>.Deregister(_moveEventBinding);
            EventBus<InteractEvent>.Deregister(_interactEventBinding);
            _moveEventBinding = null;
            _interactEventBinding = null;
        }

        protected virtual Transform CreateHighlighter()
        {
            return null;
        }

        protected virtual SelectionContext GetClampedSelection(Vector3 pos)
        {
            return new SelectionContext();
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
            CellHighlighter.position = newHover.Position;
            EventBus<HoverEvent>.Raise(new HoverEvent(newHover));
        }

        private static IEnumerator InvokeSelectionEvent()
        {
            yield return new WaitForEndOfFrame();
            EventBus<SelectionEvent>.Raise(new SelectionEvent(Hovered));
        }

        private void HandleInteractEvent(InteractEvent interactEvent)
        {
            if (!_isActive) return;
            StartCoroutine(InvokeSelectionEvent());
        }

        private void HandleMoveEvent(MoveEvent moveEvent)
        {
            if (!_isActive) return;
            var worldPosition = transform.position;
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