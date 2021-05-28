using System;

namespace Sacc
{
    public struct Node
    {
        public Symbol Symbol { get; }
        public object? Payload { get; }

        public Node(Symbol symbol, object? payload)
        {
            Symbol = symbol;
            Payload = payload;
        }

        public static Node Make<T>(T payload)
        {
            return new(Symbol.Of<T>(), payload);
        }
        
        public bool Equals(Node other)
        {
            return Symbol == other.Symbol;
        }

        public override bool Equals(object? obj)
        {
            return obj is Node other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

        public static bool operator==(Node lhs, Node rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Node lhs, Node rhs)
        {
            return !(lhs == rhs);
        }
    }
}