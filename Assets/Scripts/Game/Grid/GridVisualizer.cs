using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Grid
{
    public class GridVisualizer : MonoBehaviour
    {
        private HexGrid _hexGrid;

        private void OnDrawGizmos()
        {
            if (_hexGrid == null)
            {
                if (ServiceLocator.Instance != null) _hexGrid = ServiceLocator.Instance.Get<HexGrid>();
                return;
            }

            Gizmos.color = Color.yellow;

            for (var y = 0; y < _hexGrid.GridSize.y; y++)
            {
                for (var x = 0; x < _hexGrid.GridSize.x; x++)
                {
                    var center = _hexGrid.GetFacePosition(new Coordinate2(x, y));
                    Gizmos.DrawSphere(center, 0.05f);

                    for (var i = 0; i < 6; i++)
                    {
                        Gizmos.DrawLine(
                            _hexGrid.GetVertexPosition(new Coordinate3(x, y, i)),
                            _hexGrid.GetVertexPosition(new Coordinate3(x, y, i + 1))
                            );
                    }
                }
            }
        }
    }
}