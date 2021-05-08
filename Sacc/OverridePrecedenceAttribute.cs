using System;

namespace Sacc
{
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class OverridePrecedenceAttribute : Attribute
    {
        public Symbol PrecedenceSymbol { get; }
        
        public OverridePrecedenceAttribute(Type precedenceSymbol)
        {
            PrecedenceSymbol = new Symbol(precedenceSymbol);
        }
    }
}