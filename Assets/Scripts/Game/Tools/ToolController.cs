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
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Tools
{
    public class ToolController : MonoBehaviour, IDisposable
    {
        private const string UseToolSoundID = "Audio/SFX/place";
        private const string UseToolVfxID = "useTool";
        
        private int _radius;
        private Dictionary<SelectionType, Selector> _selectors;
        private EventBinding<SelectionEvent> _selectionEventBinding;
        private EventBinding<GamePauseEvent> _pauseEventBinding;
        private EventBinding<GameResumeEvent> _resumeEventBinding;
        private Selector _currentSelector;
        private HexController _hexController;
        public ITool[] Tools { get; private set; }
        public ITool CurrentTool { get; private set; }

        public int GetCurrentToolIndex()
        {
            for (var i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == CurrentTool) return i;
            }
            return -1;
        }
        
        public void Initialize()
        {
            _pauseEventBinding = new EventBinding<GamePauseEvent>(HandlePauseEvent);
            _resumeEventBinding = new EventBinding<GameResumeEvent>(HandleResumeEvent);
            EventBus<GamePauseEvent>.Register(_pauseEventBinding);
            EventBus<GameResumeEvent>.Register(_resumeEventBinding);
            
            _selectionEventBinding = new EventBinding<SelectionEvent>(HandleInteractEvent);
            EventBus<SelectionEvent>.Register(_selectionEventBinding);
            
            Tools = new ITool[]
            {
                new RaiseTerrain(),
                new LowerTerrain(),
                new LevelTerrain(),
                new AddMountain(),
                new AddTrees(),
                new AddFarm(),
                new AddRiver(),
                new AddPath()
            };
            
            _selectors = new Dictionary<SelectionType, Selector>
            {
                { SelectionType.Vertex, gameObject.AddComponent<VertexSelector>() },
                { SelectionType.Edge, gameObject.AddComponent<EdgeSelector>() },
                { SelectionType.Face, gameObject.AddComponent<FaceSelector>() }
            };
            
            _hexController = ServiceLocator.Instance.Get<HexController>();
            
            foreach (var selector in _selectors.Values)
            {
                selector.Initialize(_hexController);
            }
            
            SetActiveTool(0);
            
            ServiceLocator.Instance.Get<AudioController>().RegisterSound(UseToolSoundID, Resources.Load<AudioClip>(UseToolSoundID));
            ServiceLocator.Instance.Get<VFXController>().RegisterVFX(UseToolVfxID);
        }

        public void Dispose()
        {
            Tools = null;
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
            Assert.IsTrue(toolIndex >= 0 && toolIndex < Tools.Length);
            CurrentTool = Tools[toolIndex];

            SetActiveSelector(CurrentTool.SelectionType);
        }

        public void SetToolRadius(int radius)
        {
            _radius = radius;
        }

        private void HandleInteractEvent()
        {
            Assert.IsNotNull(CurrentTool);
            UseToolOnCoordinatePlusRadius(Selector.Hovered.Coordinates, _radius, CurrentTool);
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

        private void UseToolOnCoordinatePlusRadius(HashSet<CubicCoordinate> coordinates, int radius, ITool tool)
        {
            if (coordinates.Count == 0) return;
            var firstSelected = coordinates.FirstOrDefault();
            SetLevelFromCoordinate(firstSelected);
            
            if (tool.UseRadius && radius > 0) // This will only happen with face selection ie. raise / lower / level tools
            {
                var neighbourCoordinates = HexGrid.GetHexCoordinatesWithinRadius(firstSelected, radius);
                var hexes = _hexController.GetHexObjects(neighbourCoordinates, tool.CreateHexesAsNeeded);
                StartCoroutine(UseTool(tool, hexes, UnityEngine.Random.Range(0, 0.2f)));
            }
            else
            {
                var hexObjects = _hexController.GetHexObjects(coordinates, tool.CreateHexesAsNeeded);
                StartCoroutine(UseTool(tool, hexObjects));
            }
        }

        private IEnumerator UseTool(ITool tool, HexObject[] hexObjects, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            foreach (var hexObject in hexObjects)
            {
                tool.Use(hexObject);
                EventBus<PlaySoundEvent>.Raise(
                    new PlaySoundEvent(UseToolSoundID, true));
                EventBus<PlayVFXBurstEvent>.Raise(
                    new PlayVFXBurstEvent(UseToolVfxID, hexObject.Face.Position, Vector3.zero));
            }
        }
        
        private void SetLevelFromCoordinate(CubicCoordinate coordinate)
        {
            var height = ServiceLocator.Instance.Get<HexController>().GetHexHeight(coordinate);
            foreach (var tool in Tools)
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