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

        public static readonly CubicCoordinate[] Neighbours =
        {
            new (1,-1,0),
            new (1,0,-1),
            new (0,1,-1),
            new (-1,1,0),
            new (-1,0,1),
            new (0,-1,1)
        };

        public CubicCoordinate Neighbor(int index) => this + Neighbours[index % 6];

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