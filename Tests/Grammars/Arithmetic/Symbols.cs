using Sacc;

namespace Tests.Grammars.Arithmetic
{
    public static class Symbols
    {
        [Associativity(Associativity.Right)]
        public class OpExp
        {
        }
        
        [Associativity(Associativity.Left)]
        public class OpAdd
        {
        }
        
        [Associativity(Associativity.Left)]
        public class OpMinus
        {
        }
        
        public class OpUnaryMinus
        {
        }
        
        [Associativity(Associativity.Left)]
        public class OpMult
        {
        }
        
        [Associativity(Associativity.Left)]
        public class OpDiv
        {
        }

        public class LParen
        {
        }

        public class RParen
        {
        }

        public class Numeric : Expr
        {
            public int Val { get; }

            public Numeric(int val)
            {
                Val = val;
            }

            public override int Eval()
            {
                return Val;
            }
        }
        
        [StartSymbol]
        public abstract class Expr
        {
            public abstract int Eval();

            public abstract class BinaryExpr : Expr
            {
                public Expr Lhs { get; }
                public Expr Rhs { get; }

                protected BinaryExpr(Expr lhs, Expr rhs)
                {
                    Lhs = lhs;
                    Rhs = rhs;
                }
            }

            public class ExpExpr : BinaryExpr
            {
                public ExpExpr(Expr lhs, Expr rhs) : base(lhs, rhs)
                {
                }

                public override int Eval()
                {
                    var b = Lhs.Eval();
                    var e = Rhs.Eval();
                    var result = 1;
                    for (; e > 0; --e)
                    {
                        result *= b;
                    }

                    return result;
                }
            }

            public class AddExpr : BinaryExpr
            {
                public AddExpr(Expr lhs, Expr rhs) : base(lhs, rhs)
                {
                }

                public override int Eval()
                {
                    return Lhs.Eval() + Rhs.Eval();
                }
            }

            public class SubExpr : BinaryExpr
            {
                public SubExpr(Expr lhs, Expr rhs) : base(lhs, rhs)
                {
                }

                public override int Eval()
                {
                    return Lhs.Eval() - Rhs.Eval();
                }
            }

            public class MultExpr : BinaryExpr
            {
                public MultExpr(Expr lhs, Expr rhs) : base(lhs, rhs)
                {
                }

                public override int Eval()
                {
                    return Lhs.Eval() * Rhs.Eval();
                }
            }

            public class DivExpr : BinaryExpr
            {
                public DivExpr(Expr lhs, Expr rhs) : base(lhs, rhs)
                {
                }

                public override int Eval()
                {
                    return Lhs.Eval() / Rhs.Eval();
                }
            }

            public class UnaryMinusExpr : Expr
            {
                public Expr Rhs { get; }

                public UnaryMinusExpr(Expr rhs)
                {
                    Rhs = rhs;
                }

                public override int Eval()
                {
                    return -Rhs.Eval();
                }
            }

            [Production]
            public static Expr _Numeric(Numeric n)
            {
                return n;
            }

            [Production]
            public static Expr _Paren(LParen lp, Expr expr, RParen rp)
            {
                return expr;
            }
            
            [Production]
            public static Expr _Add(Expr lhs, OpAdd op, Expr rhs)
            {
                return new AddExpr(lhs, rhs);
            }
            
            [Production]
            public static Expr _Sub(Expr lhs, OpMinus op, Expr rhs)
            {
                return new SubExpr(lhs, rhs);
            }
            
            [Production]
            public static Expr _Mul(Expr lhs, OpMult op, Expr rhs)
            {
                return new MultExpr(lhs, rhs);
            }
            
            [Production]
            public static Expr _Div(Expr lhs, OpDiv op, Expr rhs)
            {
                return new DivExpr(lhs, rhs);
            }

            [Production]
            public static Expr _Exp(Expr lhs, OpExp op, Expr rhs)
            {
                return new ExpExpr(lhs, rhs);
            }

            [Production]
            [OverridePrecedence(typeof(OpUnaryMinus))]
            public static Expr _UnaryMinus(OpMinus op, Expr rhs)
            {
                return new UnaryMinusExpr(rhs);
            }
        }
    }
}