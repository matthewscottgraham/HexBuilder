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

            var saveData = ServiceLocator.Instance.Get<SaveDataController>().Load();
            if (saveData == null)
            {
                var gridSize = ServiceLocator.Instance.Get<HexGrid>().GridSize;
                _map = new GameObject[gridSize.x, gridSize.y];    
            }
            else
            {
                var gameData = (GameData)saveData.Value.Data;
                _map = new GameObject[gameData.Size.x, gameData.Size.y];
                CreateHexes(gameData.Map);
            }
            
            
            _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Register(_interactEventBinding);
            
            InvokeRepeating(nameof(Save), AutoSaveFrequecy, AutoSaveFrequecy);
        }

        public void OnDestroy()
        {
            Save();
            _hexFactory = null;
            ServiceLocator.Instance?.Deregister(this);
            ServiceLocator.Instance?.Get<EventBus<InteractEvent>>().Deregister(_interactEventBinding);
        }

        private void HandleInteractEvent()
        {
            ExecuteCommandOnHex(HexSelector.SelectedCell);
        }

        private void ExecuteCommandOnHex(Coordinate cell)
        {
            if (_map[cell.x, cell.y] == null) CreateNewHex(cell);
            ServiceLocator.Instance.Get<ToolController>()?.UseSelectedTool(_map[cell.x, cell.y]);
        }

        private void CreateNewHex(Coordinate cell)
        {
            var hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _map[cell.x, cell.y] = _hexFactory.CreateHex(cell, hexGrid, this.transform);
        }

        private void Save()
        {
            var saveData = new GameData(_map.GetLength(0), _map.GetLength(1));
            
            var dict = new Dictionary<Coordinate, int>();
            for (var x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    if (_map[x, y] == null) continue;
                    dict.Add(new Coordinate(x, y), (int)_map[x, y].transform.localScale.y);
                }
            }
            saveData.Map = dict;
            ServiceLocator.Instance?.Get<SaveDataController>().Save(saveData);
        }

        private void CreateHexes(Dictionary<Coordinate, int> hexes)
        {
            foreach (var coordinate in hexes.Keys)
            {
                if (coordinate.x < 0 || coordinate.x >= _map.GetLength(0) || coordinate.y < 0 ||
                    coordinate.y >= _map.GetLength(1)) continue;
                CreateNewHex(coordinate);
                _map[coordinate.x, coordinate.y].transform.localScale = new Vector3(1, hexes[coordinate], 1);
            }
        }
    }
}