using System;
using Game.Grid;

namespace Game.Tools.Paths
{
    public readonly struct Connection : IEquatable<Connection>
    {
        public readonly QuarticCoordinate A;
        public readonly QuarticCoordinate B;

        public Connection(QuarticCoordinate a, QuarticCoordinate b)
        {
            A = a; 
            B = b;
        }
        
        public static Connection SetVertexOrder(QuarticCoordinate a, QuarticCoordinate b)
        {
            return a.CompareTo(b) < 0
                ? new Connection(a, b)
                : new Connection(b, a);
        }

        public override bool Equals(object obj)
        {
            return obj is Connection other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (A.GetHashCode() * 397) ^ B.GetHashCode();
            }
        }

        public override string ToString() => $"({A} <-> {B})";

        public bool Equals(Connection other)
        {
            return A.Equals(other.A) && B.Equals(other.B);
        }
    }
}