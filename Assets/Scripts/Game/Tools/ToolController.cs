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
        private EventBinding<GamePauseEvent> _pauseEventBinding;
        private EventBinding<GameResumeEvent> _resumeEventBinding;
        private Selector _currentSelector;

        public ITool CurrentTool { get; private set; }

        public void Initialize()
        {
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
            
            _selectionEventBinding = new EventBinding<SelectionEvent>(HandleInteractEvent);
            EventBus<SelectionEvent>.Register(_selectionEventBinding);
            
            _tools = new ITool[]
            {
                new LevelTerrain(),
                new RaiseTerrain(),
                new LowerTerrain(),
                new AddMountain(),
                new AddRiver(),
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
            EventBus<GamePauseEvent>.Deregister(_pauseEventBinding);
            EventBus<GameResumeEvent>.Deregister(_resumeEventBinding);
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
            
            if (tool.AllowAreaOfEffect && areaOfEffect > 1)
            {
                var neighbours = HexGrid.GetHexCoordinatesWithinRadius(center, areaOfEffect);
                foreach (var neighbour in neighbours)
                {
                    tool.Use(Selector.Hovered, hexController.GetHexObject(neighbour, tool.CreateHexesAsNeeded));
                }
            }
            else
            {
                tool.Use(Selector.Hovered, hexController.GetHexObject(center, tool.CreateHexesAsNeeded));
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
        
        private void HandlePauseEvent()
        {
            foreach (var pair in _selectors)
            {
                pair.Value.Activate(false);
            }
        }

        private void HandleResumeEvent()
        {
            SetActiveSelector(_currentSelector.SelectionType);
        }
    }
}