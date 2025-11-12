using App.Services;
using Game.Cameras;
using Game.Grid;
using Game.Hexes;
using Game.Tools;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GridPreset gridPreset;
        [SerializeField] private Transform water;
        
        private void Awake()
        {
            if (ServiceLocator.Instance == null) return;
            
            ServiceLocator.Instance.Register(this);
            Initialize();
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance?.Deregister(typeof(HexGrid));
            ServiceLocator.Instance?.Deregister(typeof(HexController));
            ServiceLocator.Instance?.Deregister(typeof(ToolController));
            ServiceLocator.Instance?.Deregister(typeof(HexSelector));
            ServiceLocator.Instance?.Deregister(typeof(CameraController));
            ServiceLocator.Instance?.Deregister(this);
        }

        private void Initialize()
        {
            var grid = gridPreset.CreateGrid();
            ServiceLocator.Instance.Register(grid);
            
            var hexController = gameObject.AddComponent<HexController>();
            hexController.Initialize();
            ServiceLocator.Instance.Register(hexController);
            
            var toolController = gameObject.AddComponent<ToolController>();
            toolController.Initialize();
            ServiceLocator.Instance.Register(toolController);
            
            var hexSelector = new GameObject().AddComponent<HexSelector>();
            hexSelector.Initialize();
            ServiceLocator.Instance.Register(hexSelector);
            
            ServiceLocator.Instance.Register(new CameraController(Camera.main));
            
            water.transform.localScale = new Vector3(grid.WorldWidth() + 3, 1, grid.WorldHeight() + 3);
        }
    }
}