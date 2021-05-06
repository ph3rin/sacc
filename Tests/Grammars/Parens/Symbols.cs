using System.Collections.Generic;
using Sacc;

namespace Tests.Grammars.Parens
{
    [SymbolName("'('")]
    public class SymLParen
    {
    }

    [SymbolName("')'")]
    public class SymRParen
    {
    }

    [SymbolName("'a'")]
    public class SymA : Expression
    {
    }

    [StartSymbol, SymbolName("expr")]
    public class Expression
    {
        public class ConcatenatedExpression : Expression
        {
            public Expression Lhs { get; set; }
            public Expression Rhs { get; set; }

            public ConcatenatedExpression(Expression lhs, Expression rhs)
            {
                Lhs = lhs;
                Rhs = rhs;
            }
        }

        [Production]
        public static Expression SingleSymbol(SymA sym)
        {
            return sym;
        }

        [Production]
        public static Expression Parenthesized(SymLParen lp, Expression expr, SymRParen rp)
        {
            return expr;
        }

        [Production]
        public static Expression Concatenated(Expression lhs, Expression rhs)
        {
            return new ConcatenatedExpression(lhs, rhs);
        }
    }

    public static class CfgBuilderGenerator
    {
        public static CfgBuilder Generate()
        {
            return new CfgBuilder()
                .AddAllProductionsInClass<SymLParen>()
                .AddAllProductionsInClass<SymRParen>()
                .AddAllProductionsInClass<SymA>()
                .AddAllProductionsInClass<Expression>();
        }
    }

    public static class ExpectedParserStates
    {
        public static readonly HashSet<string> Start_Kernel = new()
        {
            "__START__ :== .expr, $"
        };

        public static readonly HashSet<string> Start_All = new()
        {
            "__START__ :== .expr, $",
            "expr :== .expr expr, $",
            "expr :== .'a', $",
            "expr :== .'(' expr ')', $",
            "expr :== .expr expr, 'a'",
            "expr :== .'a', 'a'",
            "expr :== .'(' expr ')', 'a'",
            "expr :== .expr expr, '('",
            "expr :== .'a', '('",
            "expr :== .'(' expr ')', '('"
        };

        public static readonly HashSet<string> Start_LParen_Kernel = new()
        {
            "expr :== '(' .expr ')', $",
            "expr :== '(' .expr ')', 'a'",
            "expr :== '(' .expr ')', '('"
        };

        public static readonly HashSet<string> Start_LParen_All = new()
        {
            "expr :== '(' .expr ')', $",
            "expr :== '(' .expr ')', 'a'",
            "expr :== '(' .expr ')', '('",

            "expr :== .'a', ')'",
            "expr :== .'(' expr ')', ')'",
            "expr :== .expr expr, ')'",

            "expr :== .expr expr, 'a'",
            "expr :== .'a', 'a'",
            "expr :== .'(' expr ')', 'a'",
            "expr :== .expr expr, '('",
            "expr :== .'a', '('",
            "expr :== .'(' expr ')', '('"
        };

        public static readonly HashSet<string> Start_A_Kernel = new()
        {
            "expr :== 'a' . , $",
            "expr :== 'a' . , 'a'",
            "expr :== 'a' . , '('"
        };
    }
}