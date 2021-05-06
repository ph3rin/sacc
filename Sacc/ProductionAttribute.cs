using System;

namespace Sacc
{
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class ProductionAttribute : Attribute
    {
    }
}