using System.Collections.Generic;
using NUnit.Framework;
using Sacc;
using Sacc.Helpers;

namespace Tests.Grammars.ABAB
{
    public class FindFirstsTests
    {
        [Test]
        public void FindFirsts()
        {
            var actual = CFGCoreGenerator.Generate().FindFirsts();
            var expected = new Dictionary<Symbol, HashSet<Symbol>>
            {
                {Symbol.Of<TermA>(), new HashSet<Symbol> {Symbol.Of<TermA>()}},
                {Symbol.Of<TermB>(), new HashSet<Symbol> {Symbol.Of<TermB>()}},
                {Symbol.Of<A>(), new HashSet<Symbol> {Symbol.Of<TermA>(), Symbol.Of<TermB>()}},
                {Symbol.Of<B>(), new HashSet<Symbol> {Symbol.Of<TermA>(), Symbol.Of<TermB>()}}
            };
            Assert.IsTrue(expected.DictionaryEquals(actual));
        }
    }
}