using System;

namespace Sacc
{
    public enum Associativity
    {
        Default = 0,
        Left = 1,
        Right = 2
    }
    
    [AttributeUsage(
        validOn: AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum,
        Inherited = false)]
    public class AssociativityAttribute : Attribute
    {
        public Associativity Associativity { get; }

        public AssociativityAttribute(Associativity associativity = Associativity.Default)
        {
            Associativity = associativity;
        }
    }
}