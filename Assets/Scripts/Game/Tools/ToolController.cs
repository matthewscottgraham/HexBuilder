using System;
using System.Collections.Generic;
using System.Linq;
using App.Events;
using App.Services;
using Game.Events;
using Game.Grid;
using Game.Hexes;
using Game.Selection;
using Game.Tools.Paths;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Tools
{
    public class ToolController : MonoBehaviour, IDisposable
    {
        private int _areaOfEffect;
        private ITool[] _tools;
        private Dictionary<SelectionType, Selector> _selectors;
        private EventBinding<SelectionEvent> _selectionEventBinding;
        private Selector _currentSelector;

        public ITool CurrentTool { get; private set; }

        public void Initialize()
        {
            _selectionEventBinding = new EventBinding<SelectionEvent>(HandleInteractEvent);
            EventBus<SelectionEvent>.Register(_selectionEventBinding);
            
            _tools = new ITool[]
            {
                new LevelTerrain(),
                new RaiseTerrain(),
                new LowerTerrain(),
                new AddMountain(),
                new AddWater(),
                new AddTrees(),
                new AddFarm(),
                new AddPath()
            };
            
            _selectors = new Dictionary<SelectionType, Selector>
            {
                { SelectionType.Vertex, gameObject.AddComponent<VertexSelector>() },
                { SelectionType.Edge, gameObject.AddComponent<EdgeSelector>() },
                { SelectionType.Face, gameObject.AddComponent<FaceSelector>() }
            };
            
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            foreach (var selector in _selectors.Values)
            {
                selector.Initialize(hexController);
            }
            
            SetActiveTool(1);
        }

        public void Dispose()
        {
            _tools = null;
            CurrentTool = null;
            _currentSelector = null;
            EventBus<SelectionEvent>.Deregister(_selectionEventBinding);
            _selectionEventBinding = null;
            foreach (var selector in _selectors.Values.ToArray())
            {
                selector.Dispose();
            }
            ServiceLocator.Instance.Deregister(this);
        }

        public void SetActiveTool(int toolIndex)
        {
            Assert.IsTrue(toolIndex >= 0 && toolIndex < _tools.Length);
            CurrentTool = _tools[toolIndex];

            SetActiveSelector(CurrentTool.SelectionType);
        }

        public void SetAreaOfEffect(int areaOfEffect)
        {
            _areaOfEffect = areaOfEffect;
        }

        private void HandleInteractEvent()
        {
            Assert.IsNotNull(CurrentTool);
            UseToolWithinAreaOfEffect(Selector.Hovered.Coordinate, _areaOfEffect, CurrentTool);
        }

        private void SetActiveSelector(SelectionType selectionType)
        {
            if (!_selectors.ContainsKey(selectionType)) _currentSelector = null;
            _currentSelector = _selectors[selectionType];
            foreach (var pair in _selectors)
            {
                pair.Value.Activate(pair.Key == _currentSelector.SelectionType);
            }
        }

        private void UseToolWithinAreaOfEffect(CubicCoordinate center, int areaOfEffect, ITool tool)
        {
            SetLevelFromCoordinate(center);
            var hexController = ServiceLocator.Instance.Get<HexController>();
            tool.Use(Selector.Hovered, hexController.GetHexObject(center, tool.CreateHexesAsNeeded));
            if (areaOfEffect <= 0) return;
            var neighbours = HexGrid.GetNeighboursInRange(center, areaOfEffect);
            foreach (var neighbour in neighbours)
            {
                tool.Use(Selector.Hovered, hexController.GetHexObject(neighbour, tool.CreateHexesAsNeeded));
            }
        }
        
        private void SetLevelFromCoordinate(CubicCoordinate coordinate)
        {
            var height = ServiceLocator.Instance.Get<HexController>().GetHexHeight(coordinate);
            foreach (var tool in _tools)
            {
                if (tool.GetType() != typeof(LevelTerrain)) continue;

                var levelTerrainTool = (LevelTerrain)tool;
                levelTerrainTool.Level = height;
            }
        }
    }
}