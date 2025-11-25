using System;
using System.Collections.Generic;
using System.Linq;
using App.Events;
using App.Services;
using Game.Events;
using Game.Hexes;
using Game.Selection;
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

            foreach (var selector in _selectors.Values)
            {
                selector.Initialize();
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
            UseToolWithinAreaOfEffect(Selector.Hovered.Cell, _areaOfEffect, CurrentTool);
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

        private void UseToolWithinAreaOfEffect(Cell cell, int areaOfEffect, ITool tool)
        {
            SetLevel(cell);
            var hexes = GetHexesWithinAreaOfEffect(cell, areaOfEffect);
            foreach (var hexCell in hexes.Keys) tool.Use(hexCell, hexes[hexCell]);
        }

        // TODO: move to HexGrid
        private static Dictionary<Cell, HexObject> GetHexesWithinAreaOfEffect(Cell center, int radius)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            var hexes = new Dictionary<Cell, HexObject>();

            var centerQ = center.X - (center.Y - (center.Y & 1)) / 2;
            var centerR = center.Y;

            for (var y = center.Y - radius; y <= center.Y + radius; y++)
            for (var x = center.X - radius; x <= center.X + radius; x++)
            {
                var q = x - (y - (y & 1)) / 2;

                var dq = q - centerQ;
                var dr = y - centerR;
                var dz = -dq - dr;

                var hexDistance = Mathf.Max(Mathf.Abs(dq), Mathf.Abs(dr), Mathf.Abs(dz));

                if (hexDistance > radius) continue;

                var worldCell = new Cell(x, y);
                hexes[worldCell] = hexController.GetHex(worldCell); // allow null
            }

            return hexes;
        }

        private void SetLevel(Cell cell)
        {
            var height = ServiceLocator.Instance.Get<HexController>().GetCellHeight(cell);
            foreach (var tool in _tools)
            {
                if (tool.GetType() != typeof(LevelTerrain)) continue;

                var levelTerrainTool = (LevelTerrain)tool;
                levelTerrainTool.Level = height;
            }
        }
    }
}