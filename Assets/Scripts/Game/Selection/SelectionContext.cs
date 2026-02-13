using System;
using System.Collections.Generic;
using Game.Grid;

namespace Game.Selection
{
    public readonly struct SelectionContext : IEquatable<SelectionContext>
    {
        public readonly SelectionType SelectionType;
        public readonly HashSet<CubicCoordinate> Coordinates;

        public SelectionContext(SelectionType selectionType, CubicCoordinate coordinate = new() )
        {
            SelectionType = selectionType;
            Coordinates = new HashSet<CubicCoordinate> { coordinate };
        }

        public SelectionContext(SelectionType selectionType, IEnumerable<CubicCoordinate> coordinates)
        {
            SelectionType = selectionType;
            Coordinates = new HashSet<CubicCoordinate>(coordinates);
        }
        
        public bool Equals(SelectionContext other)
        {
            return SelectionType == other.SelectionType && Coordinates.Equals(other.Coordinates);
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
                hashCode = (hashCode * 397) ^ Coordinates.GetHashCode();
                return hashCode;
            }
        }
    }
}