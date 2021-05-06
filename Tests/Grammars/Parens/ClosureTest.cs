using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    public class ClosureTest
    {
        [Test]
        public void FindNeighborsTest()
        {
            var cfg = CfgBuilderGenerator.Generate().Build();

            var item = new Item(
                Cfg.ExtendedStartSymbol.MakeProductionFromStartSymbol(Symbol.Of<Expression>()),
                0,
                Symbol.EndOfInput);

            var kernel = new HashSet<Item> {item};
            cfg.FindClosure(kernel);
            var actual = kernel.Select(i => i.ToString()).ToHashSet();
            var expected = new HashSet<string>
            {
                "__START__ :== .expr, $",       // Shift on expr
                "expr :== .expr expr, $",       // Shift on expr
                "expr :== .'a', $",             // Shift on a
                "expr :== .'(' expr ')', $",    // Shift on (
                "expr :== .expr expr, 'a'",
                "expr :== .'a', 'a'",
                "expr :== .'(' expr ')', 'a'",
                "expr :== .expr expr, '('",
                "expr :== .'a', '('",
                "expr :== .'(' expr ')', '('"
            };
            
            /*
             * Transition on expr:
             *  __START__ :== .expr, $   ->    shift
             *  expr :== .expr expr, $   ->    shift
             *  expr :== .'a', $         ->    shift
             */

            Assert.IsTrue(expected.SetEquals(actual));
        }
    }
}