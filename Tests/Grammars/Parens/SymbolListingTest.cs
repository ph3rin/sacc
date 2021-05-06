using System.Collections.Generic;
using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    public class SymbolListingTest
    {
        [Test]
        public void FindAllTerminals()
        {
            var cfgCore = CfgBuilderGenerator.Generate();
            var actual = cfgCore.FindAllTerminals();
            var expected = new HashSet<Symbol>
            {
                Symbol.Of<SymLParen>(),
                Symbol.Of<SymRParen>(),
                Symbol.Of<SymA>()
            };
            
            Assert.IsTrue(actual.SetEquals(expected));
        }

        [Test]
        public void ListAllSymbols()
        {
            var cfg = CfgBuilderGenerator.Generate().Build();
            var actual = cfg.AllSymbols;
            var expected = new HashSet<Symbol>
            {
                Symbol.Of<SymLParen>(),
                Symbol.Of<SymRParen>(),
                Symbol.Of<SymA>(),
                Symbol.Of<Expression>()
            };
            
            Assert.IsTrue(expected.SetEquals(actual));
        }
    }
}