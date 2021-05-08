using NUnit.Framework;
using Sacc;
using static Tests.Grammars.MinusMultDiv.Symbols;

namespace Tests.Grammars.MinusMultDiv
{
    public class ParseTest
    {
        private readonly ParseTable mTable =
            new ParseTableBuilder().BuildTableForCfg(
                new CfgBuilder()
                    .AddAllProductionsInClass<A>()
                    .AddAllProductionsInClass<Expr>()
                    .AddAllProductionsInClass<Minus>()
                    .AddAllProductionsInClass<Mult>()
                    .AddAllProductionsInClass<Div>()
                    .DeclarePrecedence(Symbol.Of<Minus>())
                    .DeclarePrecedence(Symbol.Of<Mult>(), Symbol.Of<Div>())
                    .Build());

        [Test]
        public void ThreeNumbers()
        {
            var minus = new Minus();
            var node = mTable.Parse(new[]
            {
                Node.Make(new A(1)),
                Node.Make(minus),
                Node.Make(new A(2)),
                Node.Make(minus),
                Node.Make(new A(3))
            });

            Assert.AreEqual(-4, (node.Payload as Expr)?.Eval());
        }

        [Test]
        public void MinusMultDiv()
        {
            var minus = Node.Make(new Minus());
            var mul = Node.Make(new Mult());
            var div = Node.Make(new Div());
            var node = mTable.Parse(new[]
            {
                Node.Make(new A(5)),
                minus,
                Node.Make(new A(3)),
                div,
                Node.Make(new A(2)),
                mul,
                Node.Make(new A(5))
            });
            
            Assert.AreEqual(0, (node.Payload as Expr)?.Eval());
        }
        
        [Test]
        public void MinusMultMinus()
        {
            var minus = Node.Make(new Minus());
            var mul = Node.Make(new Mult());
            var node = mTable.Parse(new[]
            {
                Node.Make(new A(5)),
                minus,
                Node.Make(new A(3)),
                mul,
                Node.Make(new A(2)),
                mul,
                Node.Make(new A(1)),
                minus,
                Node.Make(new A(-1))
            });
            
            Assert.AreEqual(0, (node.Payload as Expr)?.Eval());
        }
    }
}