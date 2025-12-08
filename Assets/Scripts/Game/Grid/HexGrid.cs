using UnityEngine;
using System.Collections.Generic;

namespace Game.Grid
{
    public class HexGrid
    {
        public static int GridRadius { get; private set; }
        private static float HexRadius => 2;
        private static float InnerRadius => HexRadius * Sqrt3Over2;

        private const float Sqrt3 = 1.7320508f; // Square root of 3
        private const float Sqrt3Over2 = 0.8660254f; // Square root of 3 * 0.5

        public HexGrid(int gridRadius)
        {
            GridRadius = gridRadius;
        }
        
        public static Vector3 GetLocalVertexPosition(int cornerIndex)
        {
            var angleDegrees = 60f * cornerIndex;
            var angleRadians = Mathf.Deg2Rad * angleDegrees;
            var x = Mathf.Sin(angleRadians) * HexRadius;
            var z = Mathf.Cos(angleRadians) * HexRadius;
            return new Vector3(x, 0f, z);
        }
        
        public static CubicCoordinate GetClosestHexCoordinate(Vector3 worldPos)
        {
            // TODO: rewrite this so that it makes more sense to me.
            // This was a copy paste. It converts world coordinates into cubic coordinates.
            // It appears to work, but I have trouble reading it.
            var fx = (Sqrt3 / 3f * worldPos.x - worldPos.z / 3f) / HexRadius;
            var fz = (2f / 3f * worldPos.z) / HexRadius;
            var fy = -fx - fz;

            var rx = Mathf.RoundToInt(fx);
            var ry = Mathf.RoundToInt(fy);
            var rz = Mathf.RoundToInt(fz);

            if (Mathf.Abs(rx - fx) > Mathf.Abs(ry - fy) && Mathf.Abs(rx - fx) > Mathf.Abs(rz - fz))
            {
                rx = -ry - rz;
            }
            else if (Mathf.Abs(ry - fy) > Mathf.Abs(rz - fz))
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new CubicCoordinate(rx, ry, rz);
        }
        
        public static Vector3 GetWorldPosition(CubicCoordinate coordinate)
        {
            var x = HexRadius * Sqrt3 * (coordinate.X + coordinate.Z / 2f);
            var z = HexRadius * 1.5f * coordinate.Z;
            return new Vector3(x, 0f, z);
        }
        
        public static IEnumerable<CubicCoordinate> GetNeighboursInRange(CubicCoordinate center, int radius)
        {
            for (var x = -radius; x <= radius; x++)
            {
                for (var y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
                {
                    var z = -x - y;
                    yield return new CubicCoordinate(center.X + x, center.Y + y, center.Z + z);
                }
            }
        }
        
        public static bool InBounds(CubicCoordinate coordinate)
        {
            return Mathf.Abs(coordinate.X) <= GridRadius &&
                   Mathf.Abs(coordinate.Y) <= GridRadius &&
                   Mathf.Abs(coordinate.Z) <= GridRadius;
        }
    }
}