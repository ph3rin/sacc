using System;

namespace Sacc
{
    [AttributeUsage(
        validOn: AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum,
        Inherited = false)]
    public class StartSymbolAttribute : Attribute
    {
    }
}