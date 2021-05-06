using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests.Grammars.Parens
{
    public class FindFirstsTest
    {
        [Test]
        public void FindFirsts()
        {
            var core = CfgBuilderGenerator.Generate();
            var actual = core.FindFirsts();
            var expected = Utils.MakeSymbolDictionary(
                new List<Type[]>
                {
                    new[] {typeof(SymLParen), typeof(SymLParen)},
                    new[] {typeof(SymRParen), typeof(SymRParen)},
                    new[] {typeof(SymA), typeof(SymA)},
                    new[] {typeof(Expression), typeof(SymLParen), typeof(SymA)}
                });
            Assert.IsTrue(actual.DictionaryEquals(expected));
        }
    }
}