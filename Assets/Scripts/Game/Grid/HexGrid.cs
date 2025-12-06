using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    public class HexGrid
    {
        public static float Radius { get; private set; }
        public static float InnerRadius => Radius * Mathf.Sqrt(3) / 2f;
        public static float VertexRadius { get; private set; }
        
        
        public static Vector2Int GridSize { get; private set; }
        public static float WorldWidth => GridSize.x * InnerRadius * 2f;
        public static float WorldHeight => GridSize.y * Radius * 1.5f;
        
        private static readonly Vector2Int[] CornerOwnerOffset =
        {
            new(0, 0),  // 0
            new(0, 0),  // 1
            new(0, 1),  // 2
            new(-1, 1), // 3
            new(-1, 0), // 4
            new(0, 0)   // 5
        };

        public HexGrid(Vector2Int gridSize, float radius = 2f, float vertexRadius = 0.6f)
        {
            GridSize = gridSize;
            Radius = radius;
            VertexRadius = vertexRadius;
        }

        public static Coordinate3 GetVertexCoordinate(Coordinate2 coordinate, int vertexIndex)
        {
            var offset = CornerOwnerOffset[vertexIndex];
            return new Coordinate3(coordinate.X + offset.x, coordinate.Y + offset.y, vertexIndex);
        }
        
        public static Coordinate3 GetEdgeCoordinate(Coordinate2 coordinate, int vertexIndex)
        {
            // TODO Untested yet. Probably doesnt work as it is the same code as the vertex coordinate
            var offset = CornerOwnerOffset[vertexIndex];
            return new Coordinate3(coordinate.X + offset.x, coordinate.Y + offset.y, vertexIndex);
        }

        public Vector3 GetFacePosition(Coordinate2 coordinate)
        {
            var offsetX = coordinate.Y % 2 == 0 ? 0f : InnerRadius;
            var x = (coordinate.X * InnerRadius * 2f) + offsetX;
            var z = coordinate.Y * Radius * 1.5f;
            return new Vector3(x, 0, z);
        }

        public Vector3 GetVertexPosition(Coordinate3 vertex)
        {
            var hexWorldPosition = GetFacePosition(vertex.GetGridCoordinate);
            return hexWorldPosition + GetLocalVertexPosition(vertex.Z);
        }

        public Vector3 GetEdgePosition(Coordinate3 edge)
        {
            var vertices = GetEdgeVertices(edge);
            var a = GetVertexPosition(vertices.A);
            var b = GetVertexPosition(vertices.B);
            return (a + b) * 0.5f;
        }


        public Coordinate2 GetClosestFaceCoordinateToPosition(Vector3 worldPosition)
        {
            var closestHexCoordinate = new Coordinate2(0, 0);
            var closestDistance = float.MaxValue;

            for (var y = 0; y < GridSize.y; y++)
            {
                for (var x = 0; x < GridSize.x; x++)
                {
                    var hexPosition = GetFacePosition(new Coordinate2(x, y));
                    var distance = (worldPosition - hexPosition).sqrMagnitude;
                    if (!(distance < closestDistance)) continue;
                    closestDistance = distance;
                    closestHexCoordinate = new Coordinate2(x, y);
                }
            }

            return closestHexCoordinate;
        }

        public Coordinate3 GetClosestVertexCoordinateToPosition(Vector3 worldPosition)
        {
            var closestHex = GetClosestFaceCoordinateToPosition(worldPosition);

            Coordinate3 closestVertex = default;
            var closestDistance = float.MaxValue;
            
            for (var vertexIndex = 0; vertexIndex < 6; vertexIndex++)
            {
                var vertex = GetVertexCoordinate(closestHex, vertexIndex);
                var vertexPos = GetVertexPosition(vertex);
                var distanceSquared = (worldPosition - vertexPos).sqrMagnitude;
                
                if (!(distanceSquared < closestDistance)) continue;
                closestDistance = distanceSquared;
                closestVertex = vertex;
            }

            return closestVertex;
        }

        public Coordinate3 GetClosestEdgeCoordinateToPosition(Vector3 worldPosition)
        {
            var closestHex = GetClosestFaceCoordinateToPosition(worldPosition);
            Coordinate3 closestEdge = default;
            var closestDistance = float.MaxValue;

            for (var edgeIndex = 0; edgeIndex < 6; edgeIndex++)
            {
                var edge = GetEdgeCoordinate(closestHex, edgeIndex);
                var edgePosition = GetEdgePosition(edge);
                var distanceSquared = (worldPosition - edgePosition).sqrMagnitude;
                if (!(distanceSquared < closestDistance)) continue;
                closestDistance = distanceSquared;
                closestEdge = edge;
            }

            return closestEdge;
        }


        public Vector3 GetLocalVertexPosition(int cornerIndex)
        {
            var angleDegrees = 60f * cornerIndex;
            var angleRadians = Mathf.Deg2Rad * angleDegrees;
            var localVertexPosition = new Vector3(Mathf.Sin(angleRadians) * Radius, 0, Mathf.Cos(angleRadians) * Radius);
            return localVertexPosition;
        }

        public bool AreVerticesAdjacent(Coordinate3 vertexA, Coordinate3 vertexB)
        {
            var positionA = GetVertexPosition(vertexA);
            var positionB = GetVertexPosition(vertexB);
            var distance = Vector3.Distance(positionA, positionB);

            // allow for some variance due to the vertices being perturbed
            return Mathf.Abs(distance - Radius) < Radius * 0.25f;
        }

        public List<Coordinate2> GetHexCoordinatesWithinRadius(Coordinate2 center, int radius)
        {
            // TODO I dont understand this any more. Refactor it.
            var results = new List<Coordinate2>();

            var centerQ = center.X - (center.Y - (center.Y & 1)) / 2;
            var centerR = center.Y;

            for (var y = center.Y - radius; y <= center.Y + radius; y++)
            {
                for (var x = center.X - radius; x <= center.X + radius; x++)
                {
                    if (x < 0 || x >= GridSize.x || y < 0 || y >= GridSize.y) continue;

                    var q = x - (y - (y & 1)) / 2;
                    var dq = q - centerQ;
                    var dr = y - centerR;
                    var dz = -dq - dr;

                    var dist = Mathf.Max(Mathf.Abs(dq), Mathf.Abs(dr), Mathf.Abs(dz));
                    if (dist <= radius) results.Add(new Coordinate2(x, y));
                }
            }

            return results;
        }

        private (Coordinate3 A, Coordinate3 B) GetEdgeVertices(Coordinate3 edge)
        {
            var vertexA = GetVertexCoordinate(edge.GetGridCoordinate, edge.Z);
            var vertexB = GetVertexCoordinate(edge.GetGridCoordinate, (edge.Z + 1) % 6);
            return (vertexA, vertexB);
        }
    }
}
