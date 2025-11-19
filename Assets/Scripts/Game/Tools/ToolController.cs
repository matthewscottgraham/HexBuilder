using System.Collections.Generic;
using App.Services;
using Game.Hexes;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Tools
{
    public class ToolController : MonoBehaviour
    {
        private ITool _currentTool;
        private int _areaOfEffect = 0;
        private ITool[] _tools;

        private void OnDestroy()
        {
            ServiceLocator.Instance?.Deregister(this);
        }

        public void Initialize()
        {
            _tools = new ITool[]
            {
                new LevelTerrain(),
                new RaiseTerrain(),
                new LowerTerrain(),
                new AddWater(),
                new AddTrees(),
                new AddFarm()
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
            foreach (var hexCell in hexes.Keys)
            {
                tool.Use(hexCell, hexes[hexCell]);
            }
        }

        private static Dictionary<Cell, GameObject> GetHexesWithinAreaOfEffect(Cell center, int radius)
        {
            var hexController = ServiceLocator.Instance.Get<HexController>();
            var hexes = new Dictionary<Cell, GameObject>();

            int centerQ = center.X - (center.Y - (center.Y & 1)) / 2;
            int centerR = center.Y;

            for (int y = center.Y - radius; y <= center.Y + radius; y++)
            {
                for (int x = center.X - radius; x <= center.X + radius; x++)
                {
                    int q = x - (y - (y & 1)) / 2;
                    int r = y;

                    int dq = q - centerQ;
                    int dr = r - centerR;
                    int dz = -dq - dr;

                    int hexDistance = Mathf.Max(Mathf.Abs(dq), Mathf.Abs(dr), Mathf.Abs(dz));

                    if (hexDistance <= radius)
                    {
                        var worldCell = new Cell(x, y);
                        hexes[worldCell] = hexController.GetHex(worldCell); // allow null
                    }
                }
            }

            return hexes;
        }

        private void SetLevel(Cell cell)
        {
            var height = ServiceLocator.Instance.Get<HexController>().GetCellHeight(cell);
            foreach (var tool in _tools)
            {
                if (tool.GetType() == typeof(LevelTerrain))
                {
                    var levelTerrainTool = (LevelTerrain)tool;
                    levelTerrainTool.Level = height;
                }
            }
        }
    }
}