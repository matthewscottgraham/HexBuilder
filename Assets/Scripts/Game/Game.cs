using App.Services;
using Game.Grid;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GridPreset gridPreset;
        
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
            
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(grid.WorldWidth() + 3, 0.1f, grid.WorldHeight() + 3);
        }
    }
}