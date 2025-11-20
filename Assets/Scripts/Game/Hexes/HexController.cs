using App.Events;
using App.SaveData;
using App.Services;
using Game.Grid;
using Game.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour, IDisposable
    {
        private const int AutoSaveFrequency = 60;
        private HexFactory _hexFactory;
        private EventBinding<InteractEvent> _interactEventBinding;
        private HexObject[,] _map;

        public void Initialize()
        {
            _hexFactory = new HexFactory();

            var saveData = ServiceLocator.Instance.Get<SaveDataController>().Load<GameData>();
            if (saveData == null)
            {
                var gridSize = ServiceLocator.Instance.Get<HexGrid>().GridSize;
                _map = new HexObject[gridSize.x, gridSize.y];
            }
            else
            {
                var gameData = saveData.Value.Data;
                _map = new HexObject[gameData.Size.X, gameData.Size.Y];
                CreateHexes(gameData.Map);
            }


            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            EventBus<InteractEvent>.Register(_interactEventBinding);

            InvokeRepeating(nameof(Save), AutoSaveFrequency, AutoSaveFrequency);
        }

        public void Dispose()
        {
            _hexFactory = null;
            ServiceLocator.Instance.Deregister(this);
            EventBus<InteractEvent>.Deregister(_interactEventBinding);
            _interactEventBinding = null;
            _map = null;
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

        public float GetCellHeight(Cell cell)
        {
            if (_map == null || !_map[cell.X, cell.Y]) return 1;
            return _map[cell.X, cell.Y].Height;
        }

        public HexObject GetHex(Cell cell)
        {
            if (_map == null || !InBounds(cell) || !_map[cell.X, cell.Y]) return null;
            return _map[cell.X, cell.Y];
        }

        public HexObject CreateNewHex(Cell cell)
        {
            if (!InBounds(cell)) return null;
            if (_map[cell.X, cell.Y] == null)
            {
                var hexGrid = ServiceLocator.Instance.Get<HexGrid>();
                _map[cell.X, cell.Y] = _hexFactory.CreateHex(cell, hexGrid, transform);
            }
            return _map[cell.X, cell.Y];
        }

        public bool InBounds(Cell cell)
        {
            return cell.X >= 0 && cell.X < _map.GetLength(0) && cell.Y >= 0 && cell.Y < _map.GetLength(1);
        }

        private void HandleInteractEvent()
        {
            ExecuteCommandOnHex(HexSelector.SelectedCell);
        }

        private void ExecuteCommandOnHex(Cell cell)
        {
            ServiceLocator.Instance.Get<ToolController>().UseSelectedTool(cell);
        }

        private void CreateHexes(List<CellEntry> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.Cell.X < 0 || entry.Cell.X >= _map.GetLength(0) || entry.Cell.Y < 0 ||
                    entry.Cell.Y >= _map.GetLength(1)) continue;
                CreateNewHex(entry.Cell);
                _map[entry.Cell.X, entry.Cell.Y].SetHeight(entry.Height);
            }
        }
    }
}