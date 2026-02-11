using UnityEngine;
using System.Collections.Generic;
using System;

namespace Game.Grid
{
    public class HexGrid
    {
        public static int GridRadius { get; private set; }
        public static float HexRadius => 2;
        private const float Sqrt3 = 1.7320508f; // Square root of 3

        public HexGrid(int gridRadius)
        {
            GridRadius = gridRadius;
        }
        
        public static Vector2 CubicTo2DSpace(CubicCoordinate coordinate)
        {
            return new Vector2(
                coordinate.x + coordinate.z * 0.5f,
                coordinate.z * 0.866f // This magic number is the square root of 3 / 2
            );
        }
        
        public static Vector3 GetLocalVertexPosition(int cornerIndex)
        {
            var angleDegrees = 60f * cornerIndex;
            var angleRadians = Mathf.Deg2Rad * angleDegrees;
            var x = Mathf.Sin(angleRadians) * HexRadius;
            var z = Mathf.Cos(angleRadians) * HexRadius;
            return new Vector3(x, 0f, z);
        }

        public static Vector3 GetLocalEdgePosition(int edgeIndex)
        {
            var p1 = GetLocalVertexPosition(edgeIndex);
            var p2 = GetLocalVertexPosition(edgeIndex + 1);
            return Vector3.Lerp(p1, p2, 0.5f);
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
            var x = HexRadius * Sqrt3 * (coordinate.x + coordinate.z / 2f);
            var z = HexRadius * 1.5f * coordinate.z;
            return new Vector3(x, 0f, z);
        }
        
        public static IEnumerable<CubicCoordinate> GetHexCoordinatesWithinRadius(CubicCoordinate center, int radius)
        {
            for (var x = -radius; x <= radius; x++)
            {
                for (var y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
                {
                    var z = -x - y;
                    yield return new CubicCoordinate(center.x + x, center.y + y, center.z + z);
                }
            }
        }
        
        public static bool InBounds(CubicCoordinate coordinate)
        {
            return Mathf.Abs(coordinate.x) <= GridRadius &&
                   Mathf.Abs(coordinate.y) <= GridRadius &&
                   Mathf.Abs(coordinate.z) <= GridRadius;
        }
        
        public static (CubicCoordinate A, CubicCoordinate B) GetNeighboursSharingVertex(CubicCoordinate center, int vertexIndex)
        {
            var neighbours = center.GetNeighbours();
            return vertexIndex switch
            {
                0 => (neighbours[4], neighbours[3]),
                1 => (neighbours[3], neighbours[2]),
                2 => (neighbours[2], neighbours[1]),
                3 => (neighbours[1], neighbours[0]),
                4 => (neighbours[0], neighbours[5]),
                5 => (neighbours[5], neighbours[4]),
                _ => throw new ArgumentOutOfRangeException(nameof(vertexIndex), vertexIndex, null)
            };
        }

        public static int GetSharedEdgeIndex(CubicCoordinate a, CubicCoordinate b)
        {
            var neighbours = CubicCoordinate.GetNeighboursRelative();
            var diff = a - b;
            for (var i = 0; i < 6; i++)
            {
                if (neighbours[i] == diff) return i;
            }
            return -1;
        }
        
        public static int GetSharedVertexIndex(CubicCoordinate a, CubicCoordinate b, CubicCoordinate c)
        {
            var edgeAB = GetSharedEdgeIndex(b, a);
            var edgeBC = GetSharedEdgeIndex(b, c);
            if (edgeAB < 0 || edgeBC < 0) return -1;
            
            var adjacent = (edgeAB + 1) % 6 == edgeBC || (edgeBC + 1) % 6 == edgeAB;
            if (!adjacent) return -1;
            
            var min = Mathf.Min(edgeAB, edgeBC);
            var max = Mathf.Max(edgeAB, edgeBC);
            
            if (min == 0 && max == 5)
                return 0;

            return max;
        }

        public static CubicCoordinate GetNeighbourSharingEdge(CubicCoordinate center, int edgeIndex)
        {
            var neighbours = center.GetNeighbours();
            return edgeIndex switch
            {
                0 => neighbours[3],
                1 => neighbours[2],
                2 => neighbours[1],
                3 => neighbours[0],
                4 => neighbours[5],
                5 => neighbours[4],
                _ => throw new ArgumentOutOfRangeException(nameof(edgeIndex), edgeIndex, null)
            };
        }

        public static (int A, int B) GetNeighbourVertexIndices(int vertexIndex)
        {
            return ((vertexIndex + 3) % 6, (vertexIndex + 5) % 6);
        }

        public static int GetNeighboursSharedEdgeIndex(int edgeIndex)
        {
            return (edgeIndex + 3) % 6;
        }
    }
}