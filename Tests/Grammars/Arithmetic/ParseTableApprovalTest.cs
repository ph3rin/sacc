using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using Sacc;

using static Tests.Grammars.Arithmetic.Symbols;

namespace Tests.Grammars.Arithmetic
{
    [UseReporter(typeof(DiffReporter))]
    public class ParseTableApprovalTest
    {
        [Test]
        public void DumpParseTable()
        {
            var builder = new ParseTableBuilder(true);
            builder.BuildTableForCfg(new CfgBuilder()
                .AddAllProductionsInClass<Expr>()
                .DeclarePrecedence(Symbol.Of<OpAdd>(), Symbol.Of<Symbols.OpMinus>())
                .DeclarePrecedence(Symbol.Of<Symbols.OpMult>(), Symbol.Of<Symbols.OpDiv>())
                .DeclarePrecedence(Symbol.Of<Symbols.OpExp>())
                .DeclarePrecedence(Symbol.Of<Symbols.OpUnaryMinus>())
                .Build());
            Approvals.Verify(builder.Dump());
        }
    }
}