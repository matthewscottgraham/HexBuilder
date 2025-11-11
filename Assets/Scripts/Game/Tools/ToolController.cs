using App.Services;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Tools
{
    public class ToolController : MonoBehaviour
    {
        private ITool[] _tools;
        private ITool _currentTool;

        public void Initialize()
        {
            _tools = new []
            {
                new RaiseTerrain() as ITool,
                new LowerTerrain(),
                new AddWater(),
                new AddTrees()
            };
            _currentTool = _tools[0];
        }

        public void SetActiveTool(int toolIndex)
        {
            Assert.IsTrue(toolIndex >= 0 && toolIndex < _tools.Length);
            _currentTool = _tools[toolIndex];
        }
        
        public void UseSelectedTool(GameObject hex)
        {
            Assert.IsNotNull(_currentTool);
            Assert.IsNotNull(hex);
            _currentTool.Use(hex);
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance?.Deregister(this);
        }
    }
}