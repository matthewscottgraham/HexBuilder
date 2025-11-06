using App.Services;
using UnityEngine;

namespace Game.Grid
{
    public class GridVisualizer : MonoBehaviour
    {
        private HexGrid _grid;

        private void OnDrawGizmos()
        {
            if (_grid == null)
            {
                if (ServiceLocator.Instance != null) _grid = ServiceLocator.Instance.Get<HexGrid>();
                return;
            }

            Gizmos.color = Color.yellow;

            for (var y = 0; y < _grid.GridSize.y; y++)
            {
                for (var x = 0; x < _grid.GridSize.x; x++)
                {
                    var center = _grid.GetHexCenter(x, y);
                    Gizmos.DrawSphere(center, 0.05f);
                    
                    for (var i = 0; i < 6; i++)
                    {
                        var a = _grid.GetCornerPosition(center, i);
                        var b = _grid.GetCornerPosition(center, (i + 1) % 6);
                        Gizmos.DrawLine(a, b);
                    }
                }
            }
        }
    }
}
