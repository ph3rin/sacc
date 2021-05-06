using System.Collections.Generic;
using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    public class FindAllTerminalsTest
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
    }
}