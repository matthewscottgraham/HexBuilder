using System;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public struct SelectionContext : IEquatable<SelectionContext>
    {
        public SelectionType SelectionType;
        public Vector3 Position;
        public Cell Cell;
        public int Edge;
        public int Vertex;

        public SelectionContext(SelectionType selectionType, Vector3? position, Cell? cell, int? edge, int? vertex)
        {
            SelectionType = selectionType;
            Position = position ?? Vector3.zero;
            Cell = cell ?? new Cell();
            Edge = edge ?? 0;
            Vertex = vertex ?? 0;
        }
        
        public bool Equals(SelectionContext other)
        {
            return SelectionType == other.SelectionType && Cell.Equals(other.Cell) && Edge == other.Edge && Vertex == other.Vertex;
        }

        public override bool Equals(object obj)
        {
            return obj is SelectionContext other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)SelectionType;
                hashCode = (hashCode * 397) ^ Cell.GetHashCode();
                hashCode = (hashCode * 397) ^ Edge;
                hashCode = (hashCode * 397) ^ Vertex;
                return hashCode;
            }
        }
    }
}