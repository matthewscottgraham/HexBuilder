using System;

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

        public CubicCoordinate(int row, int column)
        {
            X = row - ((column + (column & 1)) / 2);
            Z = column;
            Y = -X - Z;
        }

        private CubicCoordinate(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public CubicCoordinate GetNeighbourCoordinate(int index)
        {
            return index switch
            {
                0 => new CubicCoordinate(X + 1, Y - 1, Z),
                1 => new CubicCoordinate(X + 1, Y, Z - 1),
                2 => new CubicCoordinate(X, Y + 1, Z - 1),
                3 => new CubicCoordinate(X - 1, Y + 1, Z),
                4 => new CubicCoordinate(X - 1, Y, Z + 1),
                5 => new CubicCoordinate(X, Y - 1, Z + 1),
                _ => this
            };
        }

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
        
        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}