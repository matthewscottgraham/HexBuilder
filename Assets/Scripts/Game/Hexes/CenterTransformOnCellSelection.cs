using App.Events;
using App.Services;
using Game.Events;
using Game.Grid;
using UnityEngine;
using System.Collections;

namespace Game.Hexes
{
    public class CenterTransformOnCellSelection : MonoBehaviour
    {
        private EventBinding<CellSelectedEvent> _cellSelectedEventBinging;
        
        private void Awake()
        {
            _cellSelectedEventBinging = new EventBinding<CellSelectedEvent>(HandleCellSelected);
            ServiceLocator.Instance?.Get<EventBus<CellSelectedEvent>>().Register(_cellSelectedEventBinging);
        }
        private void OnDestroy()
        {
            ServiceLocator.Instance?.Get<EventBus<CellSelectedEvent>>()?.Deregister(_cellSelectedEventBinging);
        }

        private void HandleCellSelected()
        {
            transform.position = CoordinateToPosition(HexSelector.SelectedCell);
        }

        private Vector3 CoordinateToPosition(Cell cell)
        {
            return ServiceLocator.Instance.Get<HexGrid>().GetHexCenter(cell.x, cell.y);
        }
    }
}
