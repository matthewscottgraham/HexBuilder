using App.Events;
using App.SaveData;
using App.Services;
using Game.Grid;
using Game.Tools;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour
    {
        private HexFactory _hexFactory;
        private GameObject[,] _map;
        private EventBinding<InteractEvent> _interactEventBinding;
        
        private const int AutoSaveFrequecy = 60;
        
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
                //var gameData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(json);
                _map = new GameObject[gameData.Size.x, gameData.Size.y];
                CreateHexes(gameData.Map);
            }
            
            
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Register(_interactEventBinding);
            
            InvokeRepeating(nameof(Save), AutoSaveFrequecy, AutoSaveFrequecy);
        }

        public void OnDestroy()
        {
            _hexFactory = null;
            ServiceLocator.Instance?.Deregister(this);
            ServiceLocator.Instance?.Get<EventBus<InteractEvent>>().Deregister(_interactEventBinding);
        }

        public void Save()
        {
            var gameData = new GameData(_map.GetLength(0), _map.GetLength(1));
            
            var hexes = new List<CellEntry>();
            for (var x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    if (_map[x, y] == null) continue;
                    hexes.Add(new CellEntry(new Cell(x, y), (int)_map[x, y].transform.localScale.y));
                }
            }

            gameData.Map = hexes;
            ServiceLocator.Instance?.Get<SaveDataController>().Save(gameData);
        }

        private void HandleInteractEvent()
        {
            ExecuteCommandOnHex(HexSelector.SelectedCell);
        }

        private void ExecuteCommandOnHex(Cell cell)
        {
            if (_map[cell.x, cell.y] == null) CreateNewHex(cell);
            ServiceLocator.Instance.Get<ToolController>()?.UseSelectedTool(_map[cell.x, cell.y]);
        }

        private void CreateNewHex(Cell cell)
        {
            var hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _map[cell.x, cell.y] = _hexFactory.CreateHex(cell, hexGrid, this.transform);
        }

        private void CreateHexes(List<CellEntry> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.Cell.x < 0 || entry.Cell.x >= _map.GetLength(0) || entry.Cell.y < 0 ||
                    entry.Cell.y >= _map.GetLength(1)) continue;
                CreateNewHex(entry.Cell);
                _map[entry.Cell.x, entry.Cell.y].transform.localScale = new Vector3(1, entry.Height, 1);
            }
        }
    }
}