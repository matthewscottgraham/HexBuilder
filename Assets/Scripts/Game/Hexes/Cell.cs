using System;

namespace Game.Hexes
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public readonly int x;
        public readonly int y;
        
        public static bool operator ==(Cell left, Cell right) => left.Equals(right);
        public static bool operator !=(Cell left, Cell right) => !left.Equals(right);
        
        public readonly bool Equals(Cell other) => x == other.x && y == other.y;

        public readonly override bool Equals(object obj) => obj is Cell other && Equals(other);

        public readonly override int GetHashCode()
        {
            unchecked { return (x * 397) ^ y; }
        }

        public override string ToString() => $"Cell({x}, {y})";
    }
}