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
            var tableBuilder = new ParseTableBuilder();
            tableBuilder.BuildTableForCfg(mCfg);
            Approvals.Verify(tableBuilder.Dump());
        }
    }
}