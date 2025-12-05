using System;
using System.Collections.Generic;
using App.Config;
using App.SaveData;
using App.Services;
using Game.Features;
using Game.Grid;
using UnityEngine;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour, IDisposable
    {
        private const int AutoSaveFrequency = 60;
        private HexFactory _hexFactory;
        private HexObject[,] _map;

        public void Initialize()
        {
            var hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _hexFactory = new HexFactory(hexGrid);
            
            LoadData();
            InvokeRepeating(nameof(SaveData), AutoSaveFrequency, AutoSaveFrequency);
        }

        public void Dispose()
        {
            _hexFactory.Dispose();
            _hexFactory = null;
            
            ServiceLocator.Instance.Deregister(this);
            _map = null;
        }

        private void LoadData()
        {
            var saveData = ServiceLocator.Instance.Get<SaveDataController>().LoadSaveSlot<GameData>(ConfigController.CurrentSaveSlot);
            if (saveData == null)
            {
                var gridSize = HexGrid.GridSize;
                _map = new HexObject[gridSize.x, gridSize.y];
            }
            else
            {
                var gameData = saveData.Value.Data;
                _map = new HexObject[gameData.Size.X, gameData.Size.Y];
                CreateHexes(gameData.Map);
            }
        }

        public void SaveData()
        {
            var gameData = new GameData(_map.GetLength(0), _map.GetLength(1));

            var hexes = new List<HexInfo>();
            for (var x = 0; x < _map.GetLength(0); x++)
            {
                for (var y = 0; y < _map.GetLength(1); y++)
                {
                    if (!_map[x, y]) continue;
                    hexes.Add(new HexInfo(new Coordinate2(x, y), (int)_map[x, y].Height, _map[x, y].FeatureType,
                        _map[x, y].FeatureVariation, _map[x ,y].FeatureRotation));
                }
            }

            gameData.Map = hexes;
            ServiceLocator.Instance?.Get<SaveDataController>().SaveWithScreenshot(this, gameData);
        }

        public float GetHexHeight(Coordinate2 coordinate)
        {
            if (_map == null || !InBounds(coordinate) || !_map[coordinate.X, coordinate.Y]) return 1;
            return _map[coordinate.X, coordinate.Y].Height;
        }

        public HexObject GetHex(Coordinate2 coordinate, bool createIfMissing = false)
        {
            if (_map == null || !InBounds(coordinate)) return null;
            if (!_map[coordinate.X, coordinate.Y] && createIfMissing)
            {
                CreateNewHex(coordinate);
            }
            return _map[coordinate.X, coordinate.Y];
        }

        public HexObject CreateNewHex(Coordinate2 coordinate)
        {
            if (!InBounds(coordinate)) return null;
            if (_map[coordinate.X, coordinate.Y] != null) return _map[coordinate.X, coordinate.Y];
            
            _map[coordinate.X, coordinate.Y] = _hexFactory.CreateHex(coordinate, transform);

            return _map[coordinate.X, coordinate.Y];
        }

        public bool InBounds(Coordinate2 coordinate)
        {
            return coordinate.X >= 0 && coordinate.X < _map.GetLength(0) && coordinate.Y >= 0 && coordinate.Y < _map.GetLength(1);
        }

        private void CreateHexes(List<HexInfo> hexInfos)
        {
            var featureFactory = ServiceLocator.Instance.Get<FeatureFactory>();
            foreach (var hexInfo in hexInfos)
            {
                if (!InBounds(hexInfo.Coordinate)) continue;
                var hexObject = CreateNewHex(hexInfo.Coordinate);
                hexObject.SetHeight(hexInfo.Height);
                var feature = featureFactory.CreateFeature(hexInfo.FeatureType, hexInfo.FeatureVariation,
                    hexInfo.FeatureRotation);
                hexObject.AddFeature(feature);
            }
        }
    }
}