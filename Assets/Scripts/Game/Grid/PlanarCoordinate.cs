using System;

namespace Game.Grid
{
    /// <summary>
    /// Used to represent a 2d coordinate. It is basically the same as a Vector2Int, but doesn't
    /// contain properties I don't want when serializing.
    /// </summary>
    public readonly struct PlanarCoordinate : IEquatable<PlanarCoordinate>
    {
        public PlanarCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly int X;
        public readonly int Y;

        public static bool operator ==(PlanarCoordinate left, PlanarCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlanarCoordinate left, PlanarCoordinate right)
        {
            return !left.Equals(right);
        }

        public bool Equals(PlanarCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is PlanarCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"Cell({X}, {Y})";
        }
    }
}