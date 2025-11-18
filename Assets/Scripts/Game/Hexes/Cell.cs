using System;

namespace Game.Hexes
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public Cell(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        
        public readonly int X;
        public readonly int Y;
        
        public static bool operator ==(Cell left, Cell right) => left.Equals(right);
        public static bool operator !=(Cell left, Cell right) => !left.Equals(right);
        
        public bool Equals(Cell other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => obj is Cell other && Equals(other);

        public override int GetHashCode()
        {
            unchecked { return (X * 397) ^ Y; }
        }

        public override string ToString() => $"Cell({X}, {Y})";
    }
}