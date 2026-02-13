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
        
        public int GetCurrentToolRadius()
        {
            if (CurrentTool == null) return _radius;
            return _radius + CurrentTool.RadiusIncrement;
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

            var coordinates = Selector.Hovered.Coordinates;
            if (coordinates.Count == 0) return;
            
            SetAverageHeight(coordinates);

            var hexObjects = _hexController.GetHexObjects(coordinates, CurrentTool.CreateHexesAsNeeded);
            StartCoroutine(UseTool(CurrentTool, hexObjects));
        }

        private void SetAverageHeight(HashSet<CubicCoordinate> coordinates)
        {
            var sum = 0;
            var count = 0;
            
            foreach (var coordinate in coordinates)
            {
                var hexObject = _hexController.GetHexObject(coordinate);
                if (hexObject == null) continue;
                sum += hexObject.Height;
                count += 1;
            }

            var height = (count <= 0) ? 1 : Mathf.CeilToInt(sum / count) + 1; // Add 1 to fudge it a bit so it feels correct
            
            foreach (var tool in Tools)
            {
                if (tool.GetType() != typeof(LevelTerrain)) continue;

                var levelTerrainTool = (LevelTerrain)tool;
                levelTerrainTool.Level = height;
            }
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

        private static IEnumerator UseTool(ITool tool, HexObject[] hexObjects, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            var selectionType = Selector.Hovered.SelectionType;
            if (selectionType is SelectionType.Vertex or SelectionType.Edge)
            {
                if (!tool.Use(hexObjects)) yield break;
                foreach (var hexObject in hexObjects)
                {
                    PlayEffects(hexObject);
                }
            }
            else
            {
                foreach (var hexObject in hexObjects)
                {
                    if (!tool.Use(hexObject)) continue;
                    PlayEffects(hexObject);
                }
            }
        }

        private static void PlayEffects(HexObject hexObject)
        {
            EventBus<PlaySoundEvent>.Raise(
                new PlaySoundEvent(UseToolSoundID, true));
            EventBus<PlayVFXBurstEvent>.Raise(
                new PlayVFXBurstEvent(UseToolVfxID, hexObject.Face.Position, Vector3.zero));
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