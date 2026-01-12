using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Audio;
using App.Events;
using App.Services;
using App.VFX;
using Game.Events;
using Game.Grid;
using Game.Hexes;
using Game.Selection;
using Game.Tools.Paths;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

namespace Game.Tools
{
    public class ToolController : MonoBehaviour, IDisposable
    {
        private const string UseToolSoundID = "Audio/SFX/place";
        private const string UseToolVfxID = "VFX/useTool";
        
        private int _radius;
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
            
            ServiceLocator.Instance.Get<AudioController>().RegisterSound(UseToolSoundID, Resources.Load<AudioClip>(UseToolSoundID));
            ServiceLocator.Instance.Get<VFXController>().RegisterVFX(UseToolVfxID, Resources.Load<VisualEffectAsset>(UseToolVfxID));
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

        public void SetToolRadius(int radius)
        {
            _radius = radius;
        }

        private void HandleInteractEvent()
        {
            Assert.IsNotNull(CurrentTool);
            UseToolOnCoordinatePlusRadius(Selector.Hovered.Coordinate, _radius, CurrentTool);
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

        private void UseToolOnCoordinatePlusRadius(CubicCoordinate center, int radius, ITool tool)
        {
            SetLevelFromCoordinate(center);
            var hexController = ServiceLocator.Instance.Get<HexController>();
            
            if (tool.UseRadius && radius > 0)
            {
                var neighbours = HexGrid.GetHexCoordinatesWithinRadius(center, radius);
                foreach (var neighbour in neighbours)
                {
                    var hexObject = hexController.GetHexObject(neighbour, tool.CreateHexesAsNeeded);
                    StartCoroutine(
                        UseTool(tool, hexObject, Selector.Hovered, UnityEngine.Random.Range(0, 0.2f)));
                }
            }
            else
            {
                var hexObject = hexController.GetHexObject(center, tool.CreateHexesAsNeeded);
                StartCoroutine(UseTool(tool, hexObject, Selector.Hovered));
            }
        }

        private IEnumerator UseTool(ITool tool, HexObject hexObject, SelectionContext selectionContext, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            tool.Use(selectionContext, hexObject);
            EventBus<PlaySoundEvent>.Raise(
                new PlaySoundEvent(UseToolSoundID, true));
            EventBus<PlayVFXBurstEvent>.Raise(
                new PlayVFXBurstEvent(UseToolVfxID, hexObject.Face.Position, Vector3.zero));
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