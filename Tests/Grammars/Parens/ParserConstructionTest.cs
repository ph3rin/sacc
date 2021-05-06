using System.Collections.Generic;
using NUnit.Framework;
using Sacc;
using static Tests.Grammars.Parens.ExpectedParserStates;

namespace Tests.Grammars.Parens
{
    public class ParserConstructionTest
    {
        private Cfg mCfg;

        [SetUp]
        public void SetUp()
        {
            mCfg = CfgBuilderGenerator.Generate().Build();
        }

        [Test]
        public void InitialParserState()
        {
            var actual = mCfg.MakeInitialParserState();
            actual.Kernel.AssertMatches(Start_Kernel);
            actual.AllItems.AssertMatches(Start_All);
        }

        [Test]
        public void ShiftStart_LParen()
        {
            var action = mCfg
                .MakeInitialParserState()
                .TransitionedOn(Symbol.Of<SymLParen>(), out var actual);

            Assert.AreEqual(ParseActionType.Shift, action.Type);
            Assert.NotNull(actual);
            actual?.Kernel.AssertMatches(Start_LParen_Kernel);
            actual?.AllItems.AssertMatches(Start_LParen_All);
        }

        [Test]
        public void ShiftStart_A_MoreTransitions()
        {
            var (state, _) = mCfg
                .MakeInitialParserState()
                .TransitionedOn(Symbol.Of<SymA>());

            Assert.NotNull(state);
            if (state is null) return;
            state.Kernel.AssertMatches(Start_A_Kernel);

            {
                var (next, action) = state.TransitionedOn(Symbol.EndOfInput);
                Assert.IsNull(next);
                Assert.AreEqual(ParseActionType.Reduce, action.Type);
                Assert.IsTrue(action.Production.Matches(Symbol.Of<Expression>(), Symbol.Of<SymA>()));
            }

            {
                var (next, action) = state.TransitionedOn(Symbol.Of<SymA>());
                Assert.IsNull(next);
                Assert.AreEqual(ParseActionType.Reduce, action.Type);
                Assert.IsTrue(action.Production.Matches(Symbol.Of<Expression>(), Symbol.Of<SymA>()));
            }
        }
    }
}