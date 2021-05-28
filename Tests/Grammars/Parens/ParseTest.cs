using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    public class ParseTest
    {
        private Cfg mCfg;
        private ParseTable mParseTable;

        [SetUp]
        public void SetUp()
        {
            mCfg = CfgBuilderGenerator.Generate().Build();
            mParseTable = new ParseTableBuilder().BuildTableForCfg(mCfg);
        }
        
        [Test]
        public void Parse_Single_A()
        {
            var input = new[]
            {
                Node.Make(new SymA())
            };

            var node = mParseTable.Parse(input);
        }

        [Test]
        public void Parse_Single_A_Parenthesized()
        {
            var input = new[]
            {
                Node.Make(new SymLParen()),
                Node.Make(new SymLParen()),
                Node.Make(new SymLParen()),
                Node.Make(new SymLParen()),
                Node.Make(new SymA()),
                Node.Make(new SymRParen()),
                Node.Make(new SymRParen()),
                Node.Make(new SymRParen()),
                Node.Make(new SymRParen()),
            };

            var node = mParseTable.Parse(input);
        }
        
        [Test]
        public void Parse_Triple_A_Parenthesized()
        {
            var input = new[]
            {
                Node.Make(new SymA()),
                Node.Make(new SymA()),
                Node.Make(new SymA()),
                Node.Make(new SymA())
            };

            var node = mParseTable.Parse(input);
        }
    }
}