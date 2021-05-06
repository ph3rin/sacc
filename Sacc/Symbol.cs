using System;
using System.Linq;
using System.Reflection;

namespace Sacc
{
    public struct Symbol
    {
        public Type StaticType { get; }

        public static readonly Symbol EndOfInput = Symbol.Of<EndOfInput>();
        
        public static readonly Symbol ExtendedStartSymbol = Symbol.Of<Cfg.ExtendedStartSymbol>();
        
        public Symbol(Type staticType)
        {
            StaticType = staticType;
        }

        public static Symbol Of<T>()
        {
            return new(typeof(T));
        }

        public static bool operator ==(Symbol lhs, Symbol rhs) => lhs.Equals(rhs);

        public static bool operator !=(Symbol lhs, Symbol rhs) => !(lhs == rhs);

        private bool Equals(Symbol other)
        {
            return StaticType == other.StaticType;
        }

        public override bool Equals(object obj)
        {
            return obj is Symbol other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (StaticType != null ? StaticType.GetHashCode() : 0);
        }

        public override string ToString()
        {
            if (StaticType.GetCustomAttribute<SymbolNameAttribute>() is { } nameAttr)
            {
                return nameAttr.Name;
            }
            else
            {
                return StaticType.Name;
            }
        }
    }
}
