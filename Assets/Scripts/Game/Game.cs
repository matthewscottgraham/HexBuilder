using App.Services;
using Game.Grid;
using Game.Hexes;
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
            ServiceLocator.Instance?.Deregister(this);
        }

        private void Initialize()
        {
            var grid = gridPreset.CreateGrid();
            ServiceLocator.Instance.Register(grid);
            
            var hexController = gameObject.AddComponent<HexController>();
            hexController.Initialize();
            ServiceLocator.Instance.Register(hexController);
            
            water.transform.localScale = new Vector3(grid.WorldWidth() + 3, 0.1f, grid.WorldHeight() + 3);
        }
    }
}