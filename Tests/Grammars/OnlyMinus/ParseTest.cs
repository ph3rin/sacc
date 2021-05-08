using NUnit.Framework;
using Sacc;

using static Tests.Grammars.OnlyMinus.Symbols;

namespace Tests.Grammars.OnlyMinus
{
    public class ParseTest
    {
        [Test]
        public void ThreeNumbers()
        {
            var cfg =
                new CfgBuilder()
                    .AddAllProductionsInClass<A>()
                    .AddAllProductionsInClass<Expr>()
                    .AddAllProductionsInClass<Minus>()
                    .Build();
            var table = new ParseTableBuilder()
                .BuildTableForCfg(cfg);
            var minus = new Minus();
            var node = table.Parse(new[]
            {
                Node.Make(new A(1)),
                Node.Make(minus),
                Node.Make(new A(2)),
                Node.Make(minus),
                Node.Make(new A(3))
            });
            
            Assert.AreEqual(-4, (node.Payload as Expr)?.Eval());
        }
    }
}