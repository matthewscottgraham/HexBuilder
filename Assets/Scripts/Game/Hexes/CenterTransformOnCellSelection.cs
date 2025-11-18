using App.Events;
using App.Services;
using Game.Events;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class CenterTransformOnCellSelection : MonoBehaviour
    {
        private EventBinding<CellSelectedEvent> _cellSelectedEventBinging;
        
        private void OnEnable()
        {
            _cellSelectedEventBinging = new EventBinding<CellSelectedEvent>(HandleCellSelected);
            EventBus<CellSelectedEvent>.Register(_cellSelectedEventBinging);
        }
        private void OnDisable()
        {
            EventBus<CellSelectedEvent>.Deregister(_cellSelectedEventBinging);
            _cellSelectedEventBinging = null;
        }

        private void HandleCellSelected()
        {
            transform.position = CoordinateToPosition(HexSelector.SelectedCell);
        }

        private static Vector3 CoordinateToPosition(Cell cell)
        {
            return ServiceLocator.Instance.Get<HexGrid>().GetHexCenter(cell.X, cell.Y);
        }
    }
}
