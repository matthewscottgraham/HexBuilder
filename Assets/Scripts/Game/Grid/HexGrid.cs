using UnityEngine;

namespace Game.Grid
{
    public class HexGrid
    {
        public HexGrid(Vector2Int gridSize, Texture2D noise, int radius = 2, float noiseScale = 0.2f)
        {
            Radius = radius;
            GridSize = gridSize;
            Noise = noise;
            NoiseScale = noiseScale;
        }

        public float Radius { get; }
        public float InnerRadius => Radius * 0.866025404f;
        public Vector2Int GridSize { get; }
        public Texture2D Noise { get; }
        public float NoiseScale { get; }

        public float WorldWidth()
        {
            return GridSize.x * InnerRadius * 2;   
        }

        public float WorldHeight()
        {
            return GridSize.y * Radius * 1.5f;
        }

        public Vector3 GetClosestHexCenterToPosition(Vector3 position)
        {
            var closestDistanceSquared = Mathf.Infinity;
            var closestCenter = Vector3.zero;
    
            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    var center = GetHexCenter(x, y);
                    var distanceSquared = (position - center).sqrMagnitude;

                    if (!(distanceSquared < closestDistanceSquared)) continue;
                    closestDistanceSquared = distanceSquared;
                    closestCenter = center;
                }
            }

            return closestCenter;
        }
        
        public Vector3 GetHexCenter(int x, int y)
        {
            var offsetX = (y % 2 == 0) ? 0 : InnerRadius;
            var posX = (x * InnerRadius * 2f) + offsetX;
            var posY = y * Radius * 1.5f;

            var centerX = (GridSize.x * InnerRadius * 2f) / 2f - (InnerRadius / 2f);
            var centerY = (GridSize.y * Radius * 1.5f) / 2f - (Radius / 2f);

            return new Vector3(posX - centerX, 0, posY - centerY);
        }
        
        public Vector3 GetCornerPosition(Vector3 center, int cornerIndex)
        {
            var angleDeg = 60f * cornerIndex;
            var angleRad = Mathf.Deg2Rad * angleDeg;
            var corner = center + new Vector3(Mathf.Sin(angleRad) * Radius, 0, Mathf.Cos(angleRad) * Radius);
            corner = Perturb(corner);
            return corner;
        }
        
        private Vector3 Perturb (Vector3 position) {
            var sample = SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * NoiseScale;
            position.z += (sample.z * 2f - 1f) * NoiseScale;;
            return position;
        }

        private Vector4 SampleNoise(Vector3 position)
        {
            return Noise.GetPixelBilinear(position.x, position.z);
        }
    }
}