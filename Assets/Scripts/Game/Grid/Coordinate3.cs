using System;

namespace Game.Grid
{
    /// <summary>
    /// Used to represent 2d coordinates on a grid, with an extra integer that represents either an edge
    /// or vertex index. The edge/vertex index is in the range 0-5 and is counted clockwise from the top of a hexagon.
    /// </summary>
    public readonly struct Coordinate3 : IEquatable<Coordinate3>, IComparable<Coordinate3>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Coordinate3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z % 6; // Make sure that the Z component is always in the range 0-5.
        }
        
        public Coordinate3(Coordinate2 coordinate, int z)
        {
            X = coordinate.X;
            Y = coordinate.Y;
            Z = z % 6; // Make sure that the Z component is always in the range 0-5.
        }

        public Coordinate2 GetGridCoordinate => new Coordinate2(X, Y);

        public bool Equals(Coordinate3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public int CompareTo(Coordinate3 other)
        {
            var c = X.CompareTo(other.X);
            if (c != 0) return c;

            c = Y.CompareTo(other.Y);
            return c != 0 ? c : Z.CompareTo(other.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinate3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }
        
        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}