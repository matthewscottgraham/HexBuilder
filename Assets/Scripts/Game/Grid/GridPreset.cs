using UnityEngine;

namespace Game.Grid
{
    [CreateAssetMenu(fileName = "Grid Preset", menuName = "Grid/Preset")]
    public class GridPreset : ScriptableObject
    {
        [SerializeField] private int gridRadius = 24;

        public HexGrid CreateGrid()
        {
            return new HexGrid(gridRadius);
        }
    }
}