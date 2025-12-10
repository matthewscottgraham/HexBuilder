using System;

namespace Game.Grid
{
    /// <summary>
    /// Used to represent Cubic coordinates on a hex grid, with an extra integer (w) that represents either an edge
    /// or vertex index. The edge/vertex index is in the range 0-5 and is counted clockwise from the top of a hexagon.
    /// </summary>
    public readonly struct QuarticCoordinate : IEquatable<QuarticCoordinate>, IComparable<QuarticCoordinate>
    {
        public readonly int W;
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public QuarticCoordinate(int x, int y, int z, int w)
        {
            W = w % 6; // Make sure that the W component is always in the range 0-5.
            X = x;
            Y = y;
            Z = z;
        }
        
        public QuarticCoordinate(CubicCoordinate coordinate, int w)
        {
            W = w % 6; // Make sure that the W component is always in the range 0-5.
            X = coordinate.x;
            Y = coordinate.y;
            Z = coordinate.z;
        }

        public CubicCoordinate CubicCoordinate => new (X, Y);

        public bool Equals(QuarticCoordinate other)
        {
            return W == other. W && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is QuarticCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                hashCode = (hashCode * 397) ^ W;
                return hashCode;
            }
        }
        
        public override string ToString() => $"({X}, {Y}, {Z} : [{W}])";

        public int CompareTo(QuarticCoordinate other)
        {
            var wComparison = W.CompareTo(other.W);
            if (wComparison != 0) return wComparison;
            var xComparison = X.CompareTo(other.X);
            if (xComparison != 0) return xComparison;
            var yComparison = Y.CompareTo(other.Y);
            if (yComparison != 0) return yComparison;
            return Z.CompareTo(other.Z);
        }
    }
}