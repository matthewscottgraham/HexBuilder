using System;
using System.Collections.Generic;
using App.Config;
using App.SaveData;
using App.Services;
using Game.Features;
using Game.Grid;
using UnityEngine;
using Random = System.Random;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour, IDisposable
    {
        private const int AutoSaveFrequency = 60;
        private HexFactory _hexFactory;
        private Dictionary<CubicCoordinate, HexObject> _map;

        public void Initialize()
        {
            var hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _hexFactory = new HexFactory();
            
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

        public HexObject GetHexObject(CubicCoordinate coordinate)
        {
            if (!InBounds(coordinate)) return null;
            _map.TryGetValue(coordinate, out var o);
            return o;
        }

        public static bool InBounds(CubicCoordinate coordinate)
        {
            var col = coordinate.X + (coordinate.Z + (coordinate.Z & 1)) / 2;
            var row = coordinate.Z;

            return col >= 0 && col < HexGrid.GridSize.x
                            && row >= 0 && row < HexGrid.GridSize.y;
        }

        private void LoadData()
        {
            _map = new Dictionary<CubicCoordinate, HexObject>();
            var saveData = ServiceLocator.Instance.Get<SaveDataController>().LoadSaveSlot<GameData>(ConfigController.CurrentSaveSlot);
            if (saveData == null)
            {
                CreateRandomMap();
            }
            else
            {
                var gameData = saveData.Value.Data;
                CreateHexes(gameData.Map);
            }
        }

        public void SaveData()
        {
            var hexes = new List<HexInfo>();
            foreach (var hexObject in _map.Values)
            {
                hexes.Add(new HexInfo(hexObject));
            }
            var gameData = new GameData(HexGrid.GridSize, hexes);
            ServiceLocator.Instance?.Get<SaveDataController>().SaveWithScreenshot(this, gameData);
        }

        public float GetHexHeight(CubicCoordinate coordinate)
        {
            if (_map == null || !InBounds(coordinate) || !_map.ContainsKey(coordinate)) return 1;
            return _map[coordinate].Height;
        }

        public HexObject GetHex(CubicCoordinate coordinate, bool createIfMissing = false)
        {
            if (_map == null || !InBounds(coordinate)) return null;
            if (!_map.ContainsKey(coordinate) && createIfMissing)
            {
                CreateNewHex(coordinate);
            }
            return _map[coordinate];
        }

        public HexObject CreateNewHex(CubicCoordinate coordinate)
        {
            if (!InBounds(coordinate)) return null;
            if (!_map.ContainsKey(coordinate))
            {
                _map.Add(coordinate, _hexFactory.CreateHex(coordinate, transform));
            }
            return _map[coordinate];
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

        private void CreateRandomMap()
        {
            var weights = new int[] { 3, 2, 2, 2,  1, 0, 1, 2, 2, 2, 3 };
            var hexInfos = new List<HexInfo>();
            for (var y = 0; y < HexGrid.GridSize.y; y++)
            {
                for (var x = 0; x < HexGrid.GridSize.x; x++)
                {
                    var height = weights[UnityEngine.Random.Range(0, weights.Length)];
                    hexInfos.Add(new HexInfo(new CubicCoordinate(x, y), height, FeatureType.None, 0, 0));
                }
            }
            CreateHexes(hexInfos);
        }
    }
}