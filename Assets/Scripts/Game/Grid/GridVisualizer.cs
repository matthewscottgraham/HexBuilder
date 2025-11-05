using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Grid
{
    [RequireComponent(typeof(BoxCollider))]
    public class GridVisualizer : MonoBehaviour
    {
        private const float HexRadius = 1f;
        private const float InnerRadius = HexRadius * 0.866025404f;

        [SerializeField] private Vector2Int gridSize = new(24, 24);
        [SerializeField] private Texture2D noise;
        [SerializeField] private float noiseScale = 1f;

        private BoxCollider _collider;

        private void OnDrawGizmos()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();

            Gizmos.color = Color.yellow;

            for (var y = 0; y < gridSize.y; y++)
            {
                for (var x = 0; x < gridSize.x; x++)
                {
                    var center = GetCellPosition(x, y);
                    Gizmos.DrawSphere(center, 0.05f);
                    
                    for (int i = 0; i < 6; i++)
                    {
                        var a = GetSharedCornerPosition(x, y, i);
                        var b = GetSharedCornerPosition(x, y, (i + 1) % 6);
                        Gizmos.DrawLine(a, b);
                    }
                }
            }

            _collider.size = new Vector3(gridSize.x * InnerRadius * 2.5f, 0.1f, gridSize.y * HexRadius * 2f);
        }
        
        private Vector3 GetCellPosition(int x, int y)
        {
            var offsetX = (y % 2 == 0) ? 0 : InnerRadius;
            var posX = (x * InnerRadius * 2f) + offsetX;
            var posY = y * HexRadius * 1.5f;

            var centerX = (gridSize.x * InnerRadius * 2f) / 2f - (InnerRadius / 2f);
            var centerY = (gridSize.y * HexRadius * 1.5f) / 2f - (HexRadius / 2f);

            return new Vector3(posX - centerX, 0, posY - centerY);
        }
        
        private Vector3 GetSharedCornerPosition(int x, int y, int cornerIndex)
        {
            var center = GetCellPosition(x, y);

            var angleDeg = 60f * cornerIndex;
            var angleRad = Mathf.Deg2Rad * angleDeg;
            var corner = center + new Vector3(Mathf.Sin(angleRad) * HexRadius, 0, Mathf.Cos(angleRad) * HexRadius);
            corner = Perturb(corner);
            return corner;
        }
        
        private Vector3 Perturb (Vector3 position) {
            var sample = SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * noiseScale;
            position.z += (sample.z * 2f - 1f) * noiseScale;;
            return position;
        }

        private Vector4 SampleNoise(Vector3 position)
        {
            return noise.GetPixelBilinear(position.x, position.z);
        }
    }
}
