using UnityEngine;

namespace Game.Grid
{
    [CreateAssetMenu(fileName = "Grid Preset", menuName = "Grid/Preset")]
    public class GridPreset : ScriptableObject
    {
        [SerializeField] private Vector2Int gridSize = new(24, 24);

        public HexGrid CreateGrid()
        {
            return new HexGrid(gridSize);
        }
    }
}