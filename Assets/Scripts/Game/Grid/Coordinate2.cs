using System;

namespace Game.Grid
{
    /// <summary>
    /// Used to represent a 2d coordinate in a grid. It is basically the same as a Vector2Int, but doesnt
    /// contain properties I dont want when serializing.
    /// </summary>
    public readonly struct Coordinate2 : IEquatable<Coordinate2>
    {
        public Coordinate2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly int X;
        public readonly int Y;

        public static bool operator ==(Coordinate2 left, Coordinate2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coordinate2 left, Coordinate2 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Coordinate2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinate2 other && Equals(other);
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