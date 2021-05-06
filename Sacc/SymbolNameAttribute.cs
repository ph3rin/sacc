using System;

namespace Sacc
{
    [AttributeUsage(
        validOn: AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum,
        Inherited = false)]
    public class SymbolNameAttribute : Attribute
    {
        public string Name { get; }

        public SymbolNameAttribute(string name)
        {
            Name = name;
        }
    }
}