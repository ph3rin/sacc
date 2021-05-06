using System.Collections.Generic;
using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.ABAB
{
    public class FindAllTerminalsTest
    {
        [Test]
        public void FindAllTerminals()
        {
            var cfgCore = CFGCoreGenerator.Generate();
            var actual = cfgCore.FindAllTerminals();
            var expected = new HashSet<Symbol>
            {
                Symbol.Of<TermA>(),
                Symbol.Of<TermB>()
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }
    }
}