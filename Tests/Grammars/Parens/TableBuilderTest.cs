using ApprovalTests;
using ApprovalTests.Reporters;
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
                .Build();
            
            var tableBuilder = new ParseTableBuilder(true);
            tableBuilder.BuildTableForCfg(cfg);
            Approvals.Verify(tableBuilder.Dump());
        }
    }
}