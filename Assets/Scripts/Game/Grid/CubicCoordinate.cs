using System;
using UnityEngine;

namespace Game.Grid
{
    /// <summary>
    /// Used to represent a Cubic coordinates on a hex grid. It is basically the same as a Vector3Int, but doesn't
    /// contain properties I don't want when serializing.
    /// </summary>
    [Serializable]
    public readonly struct CubicCoordinate : IEquatable<CubicCoordinate>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public CubicCoordinate(int x, int y, int z)
        {
            if (x + y + z != 0) throw new ArgumentException("Cubic coordinates must satisfy x + y + z = 0");

            X = x;
            Y = y;
            Z = z;
        }
        
        public CubicCoordinate(int x, int z)
        {
            X = x;
            Z = z;
            Y = -x - z;
        }

        public CubicCoordinate[] GetNeighbours()
        {
            return new[]
            {
                new CubicCoordinate(X + 0, Y + 1, Z - 1), // NW
                new CubicCoordinate(X + 1, Y + 0, Z - 1), // NE
                new CubicCoordinate(X + 1, Y - 1, Z + 0), // E
                new CubicCoordinate(X + 0, Y - 1, Z + 1), // SE
                new CubicCoordinate(X - 1, Y + 0, Z + 1), // SW
                new CubicCoordinate(X - 1, Y + 1, Z + 0), // W
            };
        }

        public CubicCoordinate Neighbor(int index) => this + GetNeighbours()[index % 6];

        public static int Distance(CubicCoordinate a, CubicCoordinate b)
        {
            return Mathf.Max(Mathf.Abs(a.X - b.X), Mathf.Abs(a.Y - b.Y), Mathf.Abs(a.Z - b.Z));
        }

        public override string ToString() => $"({X}, {Y}, {Z})";

        public static CubicCoordinate operator +(CubicCoordinate a, CubicCoordinate b)
            => new (a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public bool Equals(CubicCoordinate other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is CubicCoordinate other && Equals(other);
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
    }
}