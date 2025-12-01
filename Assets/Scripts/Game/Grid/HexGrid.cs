using System.Collections.Generic;
using App.Services;
using Game.Hexes;
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
        private Texture2D Noise { get; }
        private float NoiseScale { get; }

        private static readonly Vector2Int[] CornerOwnerOffset = {
            new(0,0),  // corner 0 owned by (x,y)
            new(0,0),
            new(0,1),  // corner 2 owned by (x,y+1)
            new(-1,1), // corner 3 owned by (x-1,y+1)
            new(-1,0), // corner 4 owned by (x-1,y)
            new(0,0)
        };

        public float WorldWidth()
        {
            return GridSize.x * InnerRadius * 2;
        }

        public float WorldHeight()
        {
            return GridSize.y * Radius * 1.5f;
        }

        public List<Cell> GetCellsWithinRadius(Cell center, int radius)
        {
            var hexes = new List<Cell>();

            var centerQ = center.X - (center.Y - (center.Y & 1)) / 2;
            var centerR = center.Y;

            for (var y = center.Y - radius; y <= center.Y + radius; y++)
            {
                for (var x = center.X - radius; x <= center.X + radius; x++)
                {
                    var q = x - (y - (y & 1)) / 2;

                    var dq = q - centerQ;
                    var dr = y - centerR;
                    var dz = -dq - dr;

                    var hexDistance = Mathf.Max(Mathf.Abs(dq), Mathf.Abs(dr), Mathf.Abs(dz));

                    if (hexDistance > radius) continue;

                    hexes.Add(new Cell(x, y));
                }
            }

            return hexes;
        }

        public Cell GetClosestCellToPosition(Vector3 worldPosition)
        {
            var closestDistanceSquared = Mathf.Infinity;
            var closestCell = new Cell(0, 0);

            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    var center = GetHexWorldPosition(x, y);
                    var distanceSquared = (worldPosition - center).sqrMagnitude;

                    if (!(distanceSquared < closestDistanceSquared)) continue;
                    closestDistanceSquared = distanceSquared;
                    closestCell = new Cell(x, y);
                }
            }

            return closestCell;
        }


        public Vector3 ClampWorldPositionToHexCenter(Vector3 worldPosition)
        {
            var closestHexDistanceSquared = Mathf.Infinity;
            var closestCenter = Vector3.zero;

            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    var center = GetHexWorldPosition(x, y);
                    var distanceSquared = (worldPosition - center).sqrMagnitude;

                    if (!(distanceSquared < closestHexDistanceSquared)) continue;
                    closestHexDistanceSquared = distanceSquared;
                    closestCenter = center;
                }
            }

            return closestCenter;
        }

        public (Vector3, int) ClampWorldPositionToEdge(Vector3 worldPosition)
        {
            var hexCenter = ClampWorldPositionToHexCenter(worldPosition);

            var nearestEdgeIndex = 0;
            var bestDist = float.MaxValue;

            for (var i = 0; i < 6; i++)
            {
                var a = GetHexRelativeCornerPosition(i);
                var b = GetHexRelativeCornerPosition((i + 1) % 6);

                var edgeWorld = hexCenter + (a + b) * 0.5f;
                var dist = (worldPosition - edgeWorld).sqrMagnitude;

                if (!(dist < bestDist)) continue;
                
                bestDist = dist;
                nearestEdgeIndex = i;
            }

            var bestEdgeWorld =
                hexCenter + (GetHexRelativeCornerPosition(nearestEdgeIndex)
                             + GetHexRelativeCornerPosition((nearestEdgeIndex + 1) % 6)) * 0.5f;

            return (bestEdgeWorld, nearestEdgeIndex);
        }

        public (Vector3, int) ClampWorldPositionToVertex(Vector3 worldPosition)
        {
            var hexCenter = ClampWorldPositionToHexCenter(worldPosition);

            var nearestCornerIndex = 0;
            var bestDist = float.MaxValue;

            for (var i = 0; i < 6; i++)
            {
                var cornerWorld = hexCenter + GetHexRelativeCornerPosition(i);
                var dist = (worldPosition - cornerWorld).sqrMagnitude;

                if (!(dist < bestDist)) continue;
                
                bestDist = dist;
                nearestCornerIndex = i;
            }

            var bestCornerWorld = hexCenter + GetHexRelativeCornerPosition(nearestCornerIndex);
            return (bestCornerWorld, nearestCornerIndex);
        }

        public Vector3 GetHexWorldPosition(int x, int y)
        {
            return GetHexWorldPosition(new Cell(x, y));
        }

        public Vector3 GetHexWorldPosition(Cell cell)
        {
            var offsetX = cell.Y % 2 == 0 ? 0 : InnerRadius;
            var posX = cell.X * InnerRadius * 2f + offsetX;
            var posY = cell.Y * Radius * 1.5f;

            var centerX = GridSize.x * InnerRadius * 2f / 2f - InnerRadius / 2f;
            var centerY = GridSize.y * Radius * 1.5f / 2f - Radius / 2f;

            return Perturb(new Vector3(posX - centerX, 0, posY - centerY));
        }

        public Vector3 GetHexRelativeCornerPosition(int cornerIndex)
        {
            var angleDeg = 60f * cornerIndex;
            var angleRad = Mathf.Deg2Rad * angleDeg;
            var corner =  Perturb(new Vector3(Mathf.Sin(angleRad) * Radius, 0, Mathf.Cos(angleRad) * Radius));
            return corner;
        }

        public Dictionary<Vector3Int, Vector3> GetVertexPositions()
        {
            var vertices = new Dictionary<Vector3Int, Vector3>();

            for (var x = 0; x < GridSize.x; x++)
            {
                for (var y = 0; y < GridSize.y; y++)
                {
                    var worldPos = GetHexWorldPosition(x, y);

                    for (var corner = 0; corner < 6; corner++)
                    {
                        var offset = CornerOwnerOffset[corner];
                        var v = new Vector3Int(x + offset.x, y + offset.y, corner
                        );
                        
                        var pos = worldPos + GetHexRelativeCornerPosition(corner);
                        
                        vertices.TryAdd(v, pos);
                    }
                }
            }

            return vertices;
        }

        public static Vector3Int GetVertexCoordinate(Cell cell, int vertexIndex)
        {
            var offset = CornerOwnerOffset[vertexIndex];
            return new Vector3Int(cell.X + offset.x, cell.Y + offset.y, vertexIndex);
        }

        private Vector3 GetHexRelativeEdgeCenterPosition(int edgeIndex)
        {
            var posA = GetHexRelativeCornerPosition(edgeIndex);
            var posB = GetHexRelativeCornerPosition(6 % (edgeIndex + 1));
            return Vector3.Lerp(posA, posB, 0.5f);
        }

        private Vector3 Perturb(Vector3 position)
        {
            var sample = SampleNoise(position);
            position.x += (sample.x * 2f - 1f) * NoiseScale;
            position.z += (sample.z * 2f - 1f) * NoiseScale;
            return position;
        }
        
        private Vector4 SampleNoise(Vector3 position)
        {
            return Noise.GetPixelBilinear(position.x, position.z);
        }
    }
}