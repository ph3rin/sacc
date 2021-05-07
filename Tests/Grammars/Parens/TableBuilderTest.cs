using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.TestFrameworks;
using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    [UseReporter(typeof(NUnitReporter))]
    public class TableBuilderTest
    {
        private readonly Cfg mCfg = CfgBuilderGenerator.Generate().Build();

        [Test]
        public void BuildTable()
        {
            var cfg = CfgBuilderGenerator
                .Generate()
                .DeclarePrecedence(Symbol.Of<SymA>(), Symbol.Of<Expression>())
                .Build();
            
            var tableBuilder = new ParseTableBuilder(true);
            tableBuilder.BuildTableForCfg(cfg);
            Approvals.Verify(tableBuilder.Dump());
        }
    }
}