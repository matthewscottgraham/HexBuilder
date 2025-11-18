using System;

namespace Game.Hexes
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly int X;
        public readonly int Y;

        public static bool operator ==(Cell left, Cell right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Cell other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Cell other && Equals(other);
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