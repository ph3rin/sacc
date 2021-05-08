using Sacc;

namespace Tests.Grammars.OnlyMinus
{
    public static class Symbols
    {
        [StartSymbol]
        public abstract class Expr
        {
            public abstract int Eval();

            private class Compound : Expr
            {
                public Expr Lhs { get; }
                public Expr Rhs { get; }

                public Compound(Expr lhs, Expr rhs)
                {
                    Lhs = lhs;
                    Rhs = rhs;
                }

                public override int Eval()
                {
                    return Lhs.Eval() - Rhs.Eval();
                }
            }
            
            [Production]
            public static Expr SingleA(A a)
            {
                return a;
            }

            [Production]
            public static Expr A_Sub_A(Expr lhs, Minus m, Expr rhs)
            {
                return new Compound(lhs, rhs);
            }
        }
        
        public class A : Expr
        {
            public int Val { get; }

            public A(int val)
            {
                Val = val;
            }

            public override int Eval()
            {
                return Val;
            }
        }
        
        [Associativity(Associativity.Left)]
        public class Minus
        {
        }
    }
}