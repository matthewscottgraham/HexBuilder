using UnityEngine;

namespace Game.Grid
{
    [CreateAssetMenu(fileName = "Grid Preset", menuName = "Grid/Preset")]
    public class GridPreset : ScriptableObject
    {
        [SerializeField] private Vector2Int gridSize = new(24, 24);
        [SerializeField] private Texture2D noise;

        public HexGrid CreateGrid()
        {
            return new HexGrid(gridSize, noise);
        }
    }
}