using System;
using Game.Grid;
using Game.Hexes;
using UnityEngine;

namespace Game.Selection
{
    public struct SelectionContext : IEquatable<SelectionContext>
    {
        public SelectionType SelectionType;
        public Vector3 Position;
        public Coordinate2 Coordinate;
        public int ComponentIndex;

        public SelectionContext(SelectionType selectionType, Vector3? position, Coordinate2? cell, int? componentIndex)
        {
            SelectionType = selectionType;
            Position = position ?? Vector3.zero;
            Coordinate = cell ?? new Coordinate2();
            ComponentIndex = componentIndex ?? 0;
        }
        
        public bool Equals(SelectionContext other)
        {
            return SelectionType == other.SelectionType && Coordinate.Equals(other.Coordinate) && ComponentIndex == other.ComponentIndex;
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
                hashCode = (hashCode * 397) ^ Coordinate.GetHashCode();
                hashCode = (hashCode * 397) ^ ComponentIndex;
                return hashCode;
            }
        }
    }
}