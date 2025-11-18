using System.Collections.Generic;
using App.Events;
using App.SaveData;
using App.Services;
using Game.Grid;
using Game.Tools;
using UnityEngine;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour
    {
        private const int AutoSaveFrequecy = 60;
        private HexFactory _hexFactory;
        private EventBinding<InteractEvent> _interactEventBinding;
        private GameObject[,] _map;

        public void OnDestroy()
        {
            _hexFactory = null;
            ServiceLocator.Instance?.Deregister(this);
            EventBus<InteractEvent>.Deregister(_interactEventBinding);
            _interactEventBinding = null;
        }

        public void Initialize()
        {
            _hexFactory = new HexFactory();

            var saveData = ServiceLocator.Instance.Get<SaveDataController>().Load<GameData>();
            if (saveData == null)
            {
                var gridSize = ServiceLocator.Instance.Get<HexGrid>().GridSize;
                _map = new GameObject[gridSize.x, gridSize.y];
            }
            else
            {
                var gameData = saveData.Value.Data;
                _map = new GameObject[gameData.Size.X, gameData.Size.Y];
                CreateHexes(gameData.Map);
            }


            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            EventBus<InteractEvent>.Register(_interactEventBinding);

            InvokeRepeating(nameof(Save), AutoSaveFrequecy, AutoSaveFrequecy);
        }

        public void Save()
        {
            var gameData = new GameData(_map.GetLength(0), _map.GetLength(1));

            var hexes = new List<CellEntry>();
            for (var x = 0; x < _map.GetLength(0); x++)
            for (var y = 0; y < _map.GetLength(1); y++)
            {
                if (_map[x, y] == null) continue;
                hexes.Add(new CellEntry(new Cell(x, y), (int)_map[x, y].transform.localScale.y));
            }

            gameData.Map = hexes;
            ServiceLocator.Instance?.Get<SaveDataController>().Save(this, gameData);
        }

        private void HandleInteractEvent()
        {
            ExecuteCommandOnHex(HexSelector.SelectedCell);
        }

        private void ExecuteCommandOnHex(Cell cell)
        {
            if (_map[cell.X, cell.Y] == null) CreateNewHex(cell);
            ServiceLocator.Instance.Get<ToolController>()?.UseSelectedTool(_map[cell.X, cell.Y]);
        }

        private void CreateNewHex(Cell cell)
        {
            var hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _map[cell.X, cell.Y] = _hexFactory.CreateHex(cell, hexGrid, transform);
        }

        private void CreateHexes(List<CellEntry> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.Cell.X < 0 || entry.Cell.X >= _map.GetLength(0) || entry.Cell.Y < 0 ||
                    entry.Cell.Y >= _map.GetLength(1)) continue;
                CreateNewHex(entry.Cell);
                _map[entry.Cell.X, entry.Cell.Y].transform.localScale = new Vector3(1, entry.Height, 1);
            }
        }
    }
}