using Sacc;

namespace Tests.Grammars.ABAB
{
    public class TermA
    {
    }

    public class TermB
    {
    }

    public class A
    {
        [Production]
        public A Trivial(TermA termA)
        {
            return new();
        }

        [Production]
        public A Compound(B b, TermA termA)
        {
            return new();
        }
    }

    public class B
    {
        [Production]
        public B Trivial(TermB termB)
        {
            return new();
        }

        [Production]
        public B Compound(A a, TermB b)
        {
            return new();
        }
    }

    public static class CFGCoreGenerator
    {
        public static CfgBuilder Generate()
        {
            var cfgCore = new CfgBuilder();
            cfgCore
                .AddAllProductionsInClass<A>()
                .AddAllProductionsInClass<B>()
                .AddAllProductionsInClass<TermA>()
                .AddAllProductionsInClass<TermB>();
            return cfgCore;
        }
    }
}