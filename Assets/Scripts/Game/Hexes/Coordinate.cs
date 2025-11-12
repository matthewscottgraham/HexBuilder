using System;

namespace Game.Hexes
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public int x;
        public int y;

        public bool Equals(Coordinate other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }
    }
}