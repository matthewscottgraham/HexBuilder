using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    public class HexGrid
    {
        public static float Radius { get; private set; }
        public static float InnerRadius => Radius * Sqrt3Over2;
        public static float VertexRadius { get; private set; }
        
        
        public static Vector2Int GridSize { get; private set; }
        public static float WorldWidth => GridSize.x * InnerRadius * 2f;
        public static float WorldHeight => GridSize.y * Radius * 1.5f;
        
        private static readonly float Sqrt3 = Mathf.Sqrt(3f);
        private static readonly float Sqrt3Over2 = Sqrt3 * 0.5f;
        
        public HexGrid(Vector2Int gridSize, float radius = 2f, float vertexRadius = 0.6f)
        {
            GridSize = gridSize;
            Radius = radius;
            VertexRadius = vertexRadius;
        }

        public static Vector3 GetFacePosition(CubicCoordinate coordinate)
        {
            var worldX = InnerRadius * (Sqrt3 * coordinate.X + Sqrt3Over2 * coordinate.Z);
            var worldZ = Radius * (1.5f * coordinate.Z);
            return new Vector3(worldX, 0f, worldZ);
        }

        public Vector3 GetVertexPosition(QuarticCoordinate vertex)
        {
            var hexWorldPosition = GetFacePosition(vertex.CubicCoordinate);
            return hexWorldPosition + GetLocalVertexPosition(vertex.Z);
        }

        public Vector3 GetEdgePosition(QuarticCoordinate edge)
        {
            var vertices = GetEdgeVertices(edge);
            var a = GetVertexPosition(vertices.A);
            var b = GetVertexPosition(vertices.B);
            return (a + b) * 0.5f;
        }
        
        public static CubicCoordinate GetClosestFaceCoordinateToPosition(Vector3 worldPosition)
        {
            var closestHexCoordinate = new CubicCoordinate(0, 0);
            var closestDistance = float.MaxValue;

            for (var y = 0; y < GridSize.y; y++)
            {
                for (var x = 0; x < GridSize.x; x++)
                {
                    var hexPosition = GetFacePosition(new CubicCoordinate(x, y));
                    var distance = (worldPosition - hexPosition).sqrMagnitude;
                    if (!(distance < closestDistance)) continue;
                    closestDistance = distance;
                    closestHexCoordinate = new CubicCoordinate(x, y);
                }
            }

            return closestHexCoordinate;
        }

        public QuarticCoordinate GetClosestVertexCoordinateToPosition(Vector3 worldPosition)
        {
            var closestHex = GetClosestFaceCoordinateToPosition(worldPosition);

            QuarticCoordinate closestVertex = default;
            var closestDistance = float.MaxValue;
            
            for (var vertexIndex = 0; vertexIndex < 6; vertexIndex++)
            {
                var vertex = new QuarticCoordinate(closestHex, vertexIndex);
                var vertexPos = GetVertexPosition(vertex);
                var distanceSquared = (worldPosition - vertexPos).sqrMagnitude;
                
                if (!(distanceSquared < closestDistance)) continue;
                closestDistance = distanceSquared;
                closestVertex = vertex;
            }

            return closestVertex;
        }

        public QuarticCoordinate GetClosestEdgeCoordinateToPosition(Vector3 worldPosition)
        {
            var closestHex = GetClosestFaceCoordinateToPosition(worldPosition);
            QuarticCoordinate closestEdge = default;
            var closestDistance = float.MaxValue;

            for (var edgeIndex = 0; edgeIndex < 6; edgeIndex++)
            {
                var edge = new QuarticCoordinate(closestHex, edgeIndex);
                var edgePosition = GetEdgePosition(edge);
                var distanceSquared = (worldPosition - edgePosition).sqrMagnitude;
                if (!(distanceSquared < closestDistance)) continue;
                closestDistance = distanceSquared;
                closestEdge = edge;
            }

            return closestEdge;
        }

        public static Vector3 GetLocalVertexPosition(int cornerIndex)
        {
            var angleDegrees = 60f * cornerIndex;
            var angleRadians = Mathf.Deg2Rad * angleDegrees;
            var localVertexPosition = new Vector3(Mathf.Sin(angleRadians) * Radius, 0, Mathf.Cos(angleRadians) * Radius);
            return localVertexPosition;
        }

        public bool AreVerticesAdjacent(QuarticCoordinate vertexA, QuarticCoordinate vertexB)
        {
            var positionA = GetVertexPosition(vertexA);
            var positionB = GetVertexPosition(vertexB);
            var distance = Vector3.Distance(positionA, positionB);

            // allow for some variance due to the vertices being perturbed
            return Mathf.Abs(distance - Radius) < Radius * 0.25f;
        }

        public static IEnumerable<CubicCoordinate> GetHexCoordinatesWithinRadius(CubicCoordinate center, int radius)
        {
            for (var x = -radius; x <= radius; x++)
            {
                for (var y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
                {
                    yield return new CubicCoordinate(center.X + x, center.Y + y);
                }
            }
        }

        private (QuarticCoordinate A, QuarticCoordinate B) GetEdgeVertices(QuarticCoordinate edge)
        {
            var vertexA = new QuarticCoordinate(edge.CubicCoordinate, edge.W);
            var vertexB = new QuarticCoordinate(edge.CubicCoordinate, (edge.W + 1) % 6);
            return (vertexA, vertexB);
        }
    }
}
