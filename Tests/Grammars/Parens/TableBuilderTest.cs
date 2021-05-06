using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    public class TableBuilderTest
    {
        private readonly Cfg mCfg = CfgBuilderGenerator.Generate().Build();

        [Test]
        public void Whatever()
        {
            new ParseTableBuilder().BuildTableForCfg(mCfg);
        }
    }
}