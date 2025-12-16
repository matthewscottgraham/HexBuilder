using System;
using System.Collections.Generic;
using App.Config;
using App.SaveData;
using App.Services;
using Game.Grid;
using Game.Hexes.Features;
using UnityEngine;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour, IDisposable
    {
        private const int AutoSaveFrequency = 60;
        private HexFactory _hexFactory;
        private Dictionary<CubicCoordinate, HexObject> _map;

        public void Initialize()
        {
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

        public void SaveData()
        {
            var hexes = new List<HexInfo>();
            foreach (var hexObject in _map.Values)
            {
                hexes.Add(new HexInfo(hexObject));
            }
            var gameData = new GameData(HexGrid.GridRadius, hexes);
            ServiceLocator.Instance?.Get<SaveDataController>().SaveWithScreenshot(this, gameData);
        }

        public int GetHexHeight(CubicCoordinate coordinate)
        {
            if (!HexGrid.InBounds(coordinate) || !_map.ContainsKey(coordinate)) return 1;
            return _map[coordinate].Height;
        }

        public HexObject GetHexObject(CubicCoordinate coordinate, bool createIfMissing = false)
        {
            if (!HexGrid.InBounds(coordinate)) return null;
            if (!_map.ContainsKey(coordinate) && createIfMissing)
            {
                CreateNewHex(coordinate);
            }
            return _map[coordinate];
        }

        public HexObject CreateNewHex(CubicCoordinate coordinate)
        {
            if (!HexGrid.InBounds(coordinate)) return null;
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
                if (!HexGrid.InBounds(hexInfo.Coordinate)) continue;
                var hexObject = CreateNewHex(hexInfo.Coordinate);
                hexObject.SetHeight(hexInfo.Height);

                if (hexInfo.FeatureType != FeatureType.None)
                {
                    hexObject.Face.Add(hexInfo.FeatureType, hexInfo.FeatureVariation, hexInfo.FeatureRotation);
                }

                for (int i = 0; i < hexInfo.EdgeFeatures.Length; i++)
                {
                    if (hexInfo.EdgeFeatures[i]) hexObject.Edges.Set(i, true);
                }

                for (var i = 0; i < hexInfo.VertexFeatures.Length; i++)
                {
                    if (hexInfo.VertexFeatures[i]) hexObject.Vertices.Set(i, true); 
                }
            }
        }

        private void LoadData()
        {
            _map = new Dictionary<CubicCoordinate, HexObject>();
            var loadedData = ServiceLocator.Instance.Get<SaveDataController>().LoadSaveSlot<GameData>(ConfigController.CurrentSaveSlot);
            if (loadedData == null)
            {
                //CreateRandomMap();
                CreateEmptyMap();
            }
            else
            {
                var gameData = loadedData.Value.Data;
                CreateHexes(gameData.Map);
            }
        }

        private void CreateEmptyMap()
        {
            var hexInfos = new List<HexInfo>();

            var radius = HexGrid.GridRadius;
            
            for (var x = -radius; x <= radius; x++)
            {
                for (var z = Mathf.Max(-radius, -x - radius); z <= Mathf.Min(radius, -x + radius); z++)
                {
                    hexInfos.Add(new HexInfo(new CubicCoordinate(x, z), 0, FeatureType.None, 0, 0));
                }
            }

            CreateHexes(hexInfos);
        }

        private void CreateRandomMap()
        {
            var weights = new[] { 3, 2, 2, 2, 1, 0, 1, 2, 2, 2, 3 };
            var hexInfos = new List<HexInfo>();

            var radius = HexGrid.GridRadius;
            
            for (var x = -radius; x <= radius; x++)
            {
                for (var z = Mathf.Max(-radius, -x - radius); z <= Mathf.Min(radius, -x + radius); z++)
                {
                    var height = weights[UnityEngine.Random.Range(0, weights.Length)];
                    hexInfos.Add(new HexInfo(new CubicCoordinate(x, z), height, FeatureType.None, 0, 0));
                }
            }

            CreateHexes(hexInfos);
        }
    }
}