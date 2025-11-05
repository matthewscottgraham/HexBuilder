using UnityEngine;

namespace Game.Grid
{
    [RequireComponent(typeof(BoxCollider))]
    public class GridVisualizer : MonoBehaviour
    {
        private const float InnerRadius = HexRadius * 0.866025404f;
        private const float HexRadius = 1f;
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(24, 24);
        private BoxCollider _collider;
        
        private void OnDrawGizmos()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider>();
            Gizmos.color = Color.yellow;
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    var cellPosition = GetCellPosition(x, y);
                    Gizmos.DrawSphere(cellPosition, 0.1f);
                    Gizmos.DrawLine(GetCellCorner(cellPosition, 0), GetCellCorner(cellPosition, 1));
                    Gizmos.DrawLine(GetCellCorner(cellPosition, 1), GetCellCorner(cellPosition, 2));
                    Gizmos.DrawLine(GetCellCorner(cellPosition, 2), GetCellCorner(cellPosition, 3));
                    Gizmos.DrawLine(GetCellCorner(cellPosition, 3), GetCellCorner(cellPosition, 4));
                    Gizmos.DrawLine(GetCellCorner(cellPosition, 4), GetCellCorner(cellPosition, 5));
                    Gizmos.DrawLine(GetCellCorner(cellPosition, 5), GetCellCorner(cellPosition, 0));
                }
            }

            _collider.size = new Vector3(_gridSize.x * InnerRadius * 2, 0, _gridSize.y * HexRadius * 1.5f);
            
        }

        private Vector3 GetCellPosition(int x, int y)
        {
            return new Vector3((x + y * 0.5f - y / 2) * InnerRadius * 2, 0, y * HexRadius * 1.5f)
                   - new Vector3(_gridSize.x * InnerRadius, 0, _gridSize.y * HexRadius / 1.5f)
                   + new Vector3(HexRadius / 2f, 0, -InnerRadius * 1.5f);
        }

        private Vector3 GetCellCorner(Vector3 center, int cornerIndex)
        {
            return _corners[cornerIndex] + center;
        }
        
        public static Vector3[] _corners = {
            new Vector3(0f, 0f, HexRadius),
            new Vector3(InnerRadius, 0f, 0.5f * HexRadius),
            new Vector3(InnerRadius, 0f, -0.5f * HexRadius),
            new Vector3(0f, 0f, -HexRadius),
            new Vector3(-InnerRadius, 0f, -0.5f * HexRadius),
            new Vector3(-InnerRadius, 0f, 0.5f * HexRadius)
        };
    }
}
