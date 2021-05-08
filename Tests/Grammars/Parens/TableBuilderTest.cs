using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.TestFrameworks;
using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    [UseReporter(typeof(DiffReporter))]
    public class TableBuilderTest
    {
        private readonly Cfg mCfg = CfgBuilderGenerator.Generate().Build();

        [Test]
        public void BuildTable()
        {
            var cfg = CfgBuilderGenerator
                .Generate()
                //.DeclarePrecedence()
                .DeclarePrecedence(Symbol.Of<Expression>(), Symbol.Of<SymLParen>(), Symbol.Of<SymRParen>(), Symbol.Of<SymA>())
                .Build();
            
            var tableBuilder = new ParseTableBuilder(true);
            tableBuilder.BuildTableForCfg(cfg);
            Approvals.Verify(tableBuilder.Dump());
        }
    }
}