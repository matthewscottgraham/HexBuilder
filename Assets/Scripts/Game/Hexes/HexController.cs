using System;
using System.Collections.Generic;
using App.Config;
using App.SaveData;
using App.Services;
using Game.Grid;
using Game.Hexes.Features;
using Game.Map;
using UnityEngine;

namespace Game.Hexes
{
    public class HexController : MonoBehaviour, IDisposable
    {
        private static MapType _newMapType = MapType.Random;
        
        private const int AutoSaveFrequency = 60;
        private HexFactory _hexFactory;
        private MapFactory _mapFactory;
        private Dictionary<CubicCoordinate, HexObject> _map;
        
        public WaterfallFactory WaterfallFactory { get; private set; }

        public static void SetNewMapType(MapType mapType)
        {
            _newMapType = mapType;
        }
        
        public void Initialize()
        {
            _hexFactory = new HexFactory();
            _mapFactory = new MapFactory();
            
            WaterfallFactory = new WaterfallFactory();
            
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
            ServiceLocator.Instance?.Get<SaveDataController>().SaveWithScreenshot(gameData);
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
        public HexObject[] GetHexObjects(IEnumerable<CubicCoordinate> coordinates, bool createIfMissing = false)
        {
            var hexObjects = new List<HexObject>();
            foreach (var coordinate in coordinates)
            {
                if (!HexGrid.InBounds(coordinate)) continue;
                if (!_map.ContainsKey(coordinate) && createIfMissing)
                {
                    CreateNewHex(coordinate);
                }
                hexObjects.Add(_map[coordinate]);
            }
            return hexObjects.ToArray();
        }

        private HexObject CreateNewHex(CubicCoordinate coordinate)
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
            foreach (var hexInfo in hexInfos)
            {
                if (!HexGrid.InBounds(hexInfo.Coordinate)) continue;
                var hexObject = CreateNewHex(hexInfo.Coordinate);
                hexObject.SetHeight(hexInfo.Height);

                if (hexInfo.FeatureType != FeatureType.None)
                {
                    hexObject.Face.Add(hexInfo.FeatureType, hexInfo.FeatureVariation);
                }

                for (var i = 0; i < hexInfo.EdgeFeatures.Length; i++)
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
            List<HexInfo> hexInfos;
#if UNITY_WEBGL
            hexInfos = _mapFactory.CreateMap(_newMapType);
#else
            
            var loadedData = ServiceLocator.Instance.Get<SaveDataController>().LoadSaveSlot<GameData>(ConfigController.CurrentSaveSlot);
            if (loadedData == null)
            {
                hexInfos = _mapFactory.CreateMap(_newMapType);
            }
            else
            {
                var gameData = loadedData.Value.Data;
                hexInfos = gameData.Map;
            }
#endif
            CreateHexes(hexInfos);
        }
    }
}