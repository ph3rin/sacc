using Sacc;

namespace Tests.Grammars.MinusMultDiv
{
    public static class Symbols
    {
        [StartSymbol]
        public abstract class Expr
        {
            public abstract int Eval();

            private class SubExpr : Expr
            {
                public Expr Lhs { get; }
                public Expr Rhs { get; }

                public SubExpr(Expr lhs, Expr rhs)
                {
                    Lhs = lhs;
                    Rhs = rhs;
                }

                public override int Eval()
                {
                    return Lhs.Eval() - Rhs.Eval();
                }
            }

            private class MultExpr : Expr
            {
                public Expr Lhs { get; }
                public Expr Rhs { get; }

                public MultExpr(Expr lhs, Expr rhs)
                {
                    Lhs = lhs;
                    Rhs = rhs;
                }

                public override int Eval()
                {
                    return Lhs.Eval() * Rhs.Eval();
                }
            }
            
            private class DivExpr : Expr
            {
                public Expr Lhs { get; }
                public Expr Rhs { get; }

                public DivExpr(Expr lhs, Expr rhs)
                {
                    Lhs = lhs;
                    Rhs = rhs;
                }

                public override int Eval()
                {
                    return Lhs.Eval() / Rhs.Eval();
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
                return new SubExpr(lhs, rhs);
            }

            [Production]
            public static Expr A_Mult_A(Expr lhs, Mult m, Expr rhs)
            {
                return new MultExpr(lhs, rhs);
            }

            [Production]
            public static Expr A_Div_A(Expr lhs, Div d, Expr rhs)
            {
                return new DivExpr(lhs, rhs);
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

        [Associativity(Associativity.Left)]
        public class Mult
        {
        }

        [Associativity(Associativity.Left)]
        public class Div
        {
        }
        
    }
}