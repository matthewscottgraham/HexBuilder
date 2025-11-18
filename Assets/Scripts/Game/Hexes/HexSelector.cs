using App.Events;
using App.Input;
using App.Services;
using Game.Events;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class HexSelector : MonoBehaviour
    {
        private Camera _camera;
        private Transform _cellHighlighter;

        private EventBinding<MoveEvent> _moveEventBinding;

        public static Cell SelectedCell { get; private set; }

        private void Update()
        {
            if (!InputController.PointerHasMovedThisFrame) return;

            var ray = _camera.ScreenPointToRay(InputController.PointerPosition);
            if (!Physics.Raycast(ray, out var hit)) return;

            var originalCell = SelectedCell;
            transform.position = ClampPositionToCell(hit.point);
            NotifyIfNewSelection(originalCell);
        }

        private void OnDestroy()
        {
            EventBus<MoveEvent>.Deregister(_moveEventBinding);
            _moveEventBinding = null;
        }

        public void Initialize()
        {
            _moveEventBinding = new EventBinding<MoveEvent>(HandleMoveEvent);
            EventBus<MoveEvent>.Register(_moveEventBinding);

            _camera = Camera.main;
            _cellHighlighter = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            _cellHighlighter.SetParent(transform);
            _cellHighlighter.localPosition = Vector3.up;
            _cellHighlighter.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/mat_highlight");
        }

        private static void NotifyIfNewSelection(Cell originalCell)
        {
            if (!SelectedCell.Equals(originalCell))
                EventBus<CellSelectedEvent>.Raise(new CellSelectedEvent(SelectedCell));
        }

        private void HandleMoveEvent(MoveEvent moveEvent)
        {
            var originalCell = SelectedCell;
            if (moveEvent.Delta.y != 0)
            {
                var nudge = moveEvent.Delta.y > 0 ? 1 : -1;
                transform.position = ClampPositionToCell(new Vector3(
                    transform.position.x + moveEvent.Delta.x * 2 + nudge,
                    0,
                    transform.position.z + moveEvent.Delta.y * 1.5f));
            }
            else
            {
                transform.position = ClampPositionToCell(new Vector3(
                    transform.position.x + moveEvent.Delta.x * 2,
                    0,
                    transform.position.z + moveEvent.Delta.y * 1.5f));
            }

            NotifyIfNewSelection(originalCell);
        }

        private static Vector3 ClampPositionToCell(Vector3 position)
        {
            SelectedCell = ServiceLocator.Instance.Get<HexGrid>().GetClosestCellToPosition(position);
            return ServiceLocator.Instance.Get<HexGrid>().GetClosestHexCenterToPosition(position);
        }
    }
}