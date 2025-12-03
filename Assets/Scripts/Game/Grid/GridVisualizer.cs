using App.Services;
using Game.Hexes;
using UnityEngine;

namespace Game.Grid
{
    public class GridVisualizer : MonoBehaviour
    {
        private HexGrid _hexGrid;
        private HexController _hexController;

        private void Initialise()
        {
            if (ServiceLocator.Instance == null) return;
            _hexGrid = ServiceLocator.Instance.Get<HexGrid>();
            _hexController = ServiceLocator.Instance.Get<HexController>();
        }
        private void OnDrawGizmos()
        {
            if (_hexGrid == null || _hexController == null)
            {
                 Initialise();
                return;
            }

            Gizmos.color = Color.yellow;

            for (var y = 0; y < _hexGrid.GridSize.y; y++)
            {
                for (var x = 0; x < _hexGrid.GridSize.x; x++)
                {
                    var coordinate = new Coordinate2(x, y);
                    var height = new Vector3(0, _hexController.GetHexHeight(coordinate), 0);
                    var center = _hexGrid.GetFacePosition(coordinate) + height;
                    Gizmos.DrawSphere(center, 0.05f);

                    for (var i = 0; i < 6; i++)
                    {
                        Gizmos.DrawLine(
                            _hexGrid.GetVertexPosition(new Coordinate3(x, y, i)) + height,
                            _hexGrid.GetVertexPosition(new Coordinate3(x, y, i + 1)) + height
                            );
                    }
                }
            }
        }
    }
}