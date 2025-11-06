using App.Events;
using App.Services;
using Game.Grid;
using Game.Tools;
using UnityEngine;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour
    {
        private HexFactory _hexFactory;
        private GameObject[,] _map;
        private EventBinding<InteractEvent> _interactEventBinding;
        public void Initialize()
        {
            _hexFactory = new HexFactory();
            var gridSize = ServiceLocator.Instance.Get<HexGrid>().GridSize;
            _map = new GameObject[gridSize.x, gridSize.y];
            
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Register(_interactEventBinding);
        }

        public void OnDestroy()
        {
            _hexFactory = null;
            ServiceLocator.Instance?.Deregister(this);
            ServiceLocator.Instance?.Get<EventBus<InteractEvent>>().Deregister(_interactEventBinding);
        }

        private void HandleInteractEvent()
        {
            ExecuteCommandOnHex(HexSelector.SelectedCell);
        }

        private void ExecuteCommandOnHex(Vector2Int cell)
        {
            if (_map[cell.x, cell.y] == null) CreateNewHex(cell);
            ServiceLocator.Instance.Get<ToolController>()?.UseSelectedTool(_map[cell.x, cell.y]);
        }

        private void CreateNewHex(Vector2Int cell)
        {
            var hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _map[cell.x, cell.y] = _hexFactory.CreateHex(cell, hexGrid, this.transform);
        }
    }
}