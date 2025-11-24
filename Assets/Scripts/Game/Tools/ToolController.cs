using System;
using System.Collections.Generic;
using App.Services;
using Game.Hexes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Tools
{
    public class ToolController : MonoBehaviour, IDisposable
    {
        private int _areaOfEffect;
        private ITool _currentTool;
        private ITool[] _tools;
        
        public ITool CurrentTool => _currentTool;
        
        public void Dispose()
        {
            _tools = null;
            _currentTool = null;
            ServiceLocator.Instance.Deregister(this);
        }

        public void Initialize()
        {
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
            _currentTool = _tools[1];
        }

        public void SetActiveTool(int toolIndex)
        {
            Assert.IsTrue(toolIndex >= 0 && toolIndex < _tools.Length);
            _currentTool = _tools[toolIndex];
        }

        public void SetAreaOfEffect(int areaOfEffect)
        {
            _areaOfEffect = areaOfEffect;
        }

        public void UseSelectedTool(Cell cell)
        {
            Assert.IsNotNull(_currentTool);
            UseToolWithinAreaOfEffect(cell, _areaOfEffect, _currentTool);
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