using System.Reflection;
using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Parens
{
    public class TransitionTest
    {
        private Cfg mCfg;
        private readonly MethodInfo mDummyMethodInfo = typeof(int).GetMethods()[0];
        private ProductionRule mParenthesizedProduction;

        [SetUp]
        public void SetUp()
        {
            mCfg = CfgBuilderGenerator.Generate().Build();
            mParenthesizedProduction = new ProductionRule(
                Symbol.Of<Expression>(),
                new[] {Symbol.Of<SymLParen>(), Symbol.Of<Expression>(), Symbol.Of<SymRParen>()},
                mDummyMethodInfo);
        }

        private void Run(Item item, Symbol input, Item? expectedTarget, ParseAction expectedParseAction)
        {
            var actual = mCfg.FindTransitionOfItemOn(item, input);
            Assert.AreEqual((expectedTarget, expectedParseAction), actual);
        }

        [Test]
        public void Expected_Expr_Shift()
        {
            var item = new Item(
                Cfg.ExtendedStartSymbol.MakeProductionFromStartSymbol(Symbol.Of<Expression>()),
                0,
                Symbol.EndOfInput);

            Run(item, Symbol.EndOfInput, null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymLParen>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymRParen>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<Expression>(), item.ShiftedByOne(), ParseAction.MakeShift());
        }

        [Test]
        public void Expected_Eof_Accept()
        {
            var item = new Item(
                Cfg.ExtendedStartSymbol.MakeProductionFromStartSymbol(Symbol.Of<Expression>()),
                1,
                Symbol.EndOfInput);

            Run(item, Symbol.EndOfInput, null, ParseAction.MakeAccept());
            Run(item, Symbol.Of<SymLParen>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymRParen>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<Expression>(), null, ParseAction.MakeDiscard());
        }

        [Test]
        public void Expected_LParen_Shift()
        {
            var item = new Item(mParenthesizedProduction, 0, Symbol.EndOfInput);
            
            Run(item, Symbol.EndOfInput, null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymLParen>(), item.ShiftedByOne(), ParseAction.MakeShift());
            Run(item, Symbol.Of<SymRParen>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymA>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<Expression>(), null, ParseAction.MakeDiscard());
        }
        
        [Test]
        public void Expected_A_Reduce()
        {
            var item = new Item(mParenthesizedProduction, 3, Symbol.Of<SymA>());
            
            Run(item, Symbol.EndOfInput, null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymLParen>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymRParen>(), null, ParseAction.MakeDiscard());
            Run(item, Symbol.Of<SymA>(), null, ParseAction.MakeReduce(mParenthesizedProduction));
            Run(item, Symbol.Of<Expression>(), null, ParseAction.MakeDiscard());
        }
    }
}