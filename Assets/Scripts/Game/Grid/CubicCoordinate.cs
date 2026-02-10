using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Grid
{
    /// <summary>
    /// Used to represent a Cubic coordinates on a hex grid. It is basically the same as a Vector3Int, but doesn't
    /// contain properties I don't want when serializing.
    /// </summary>
    [Serializable]
    public struct CubicCoordinate : IEquatable<CubicCoordinate>
    {
        [FormerlySerializedAs("X")] public int x;
        [FormerlySerializedAs("Y")] public int y;
        [FormerlySerializedAs("Z")] public int z;

        public CubicCoordinate(int x, int y, int z)
        {
            if (x + y + z != 0) throw new ArgumentException("Cubic coordinates must satisfy x + y + z = 0");

            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public CubicCoordinate(int x, int z)
        {
            this.x = x;
            this.z = z;
            y = -x - z;
        }

        public CubicCoordinate[] GetNeighbours()
        {
            return new[]
            {
                new CubicCoordinate(x + 0, y + 1, z - 1), // NW
                new CubicCoordinate(x + 1, y + 0, z - 1), // NE
                new CubicCoordinate(x + 1, y - 1, z + 0), // E
                new CubicCoordinate(x + 0, y - 1, z + 1), // SE
                new CubicCoordinate(x - 1, y + 0, z + 1), // SW
                new CubicCoordinate(x - 1, y + 1, z + 0), // W
            };
        }

        public CubicCoordinate Neighbor(int index) => this + GetNeighbours()[index % 6];

        public static int Distance(CubicCoordinate a, CubicCoordinate b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
        }

        public override string ToString() => $"({x}, {y}, {z})";

        private static readonly CubicCoordinate[] NeighboursRelative =
        {
            new (0, 1, -1), // NW
            new (-1, 1, 0), // W
            new (-1, 0, 1), // SW
            new (0, -1, 1), // SE
            new (1, -1, 0), // E
            new (1, 0, -1), // NE
        };
        
        public static CubicCoordinate[] GetNeighboursRelative() { return NeighboursRelative; }
        
        public static CubicCoordinate operator +(CubicCoordinate a, CubicCoordinate b)
            => new (a.x + b.x, a.y + b.y, a.z + b.z);

        public static CubicCoordinate operator -(CubicCoordinate a, CubicCoordinate b)
            => new (a.x - b.x, a.y - b.y, a.z - b.z);

        public static bool operator ==(CubicCoordinate a, CubicCoordinate b)
            => a.x == b.x && a.y == b.y && a.z == b.z;

        public static bool operator !=(CubicCoordinate a, CubicCoordinate b)
            => a.x != b.x || a.y != b.y || a.z != b.z;
        
        public bool Equals(CubicCoordinate other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is CubicCoordinate other && this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = x;
                hashCode = (hashCode * 397) ^ y;
                hashCode = (hashCode * 397) ^ z;
                return hashCode;
            }
        }
    }
}