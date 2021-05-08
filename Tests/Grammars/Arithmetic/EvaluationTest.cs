﻿using System;
using NUnit.Framework;
using Sacc;

using static Tests.Grammars.Arithmetic.Symbols;

namespace Tests.Grammars.Arithmetic
{
    public class EvaluationTest
    {
        private readonly ParseTable mTable =
            new ParseTableBuilder()
                .BuildTableForCfg(new CfgBuilder()
                    .AddAllProductionsInClass<Expr>()
                    .DeclarePrecedence(Symbol.Of<OpAdd>(), Symbol.Of<OpMinus>())
                    .DeclarePrecedence(Symbol.Of<OpMult>(), Symbol.Of<OpDiv>())
                    .DeclarePrecedence(Symbol.Of<OpExp>())
                    .DeclarePrecedence(Symbol.Of<OpUnaryMinus>())
                    .Build());

        [Test]
        public void SimpleAdd()
        {
            // 1 + 1
            var input = new[]
            {
                Node.Make(new Numeric(1)),
                Node.Make(new OpAdd()),
                Node.Make(new Numeric(1))
            };
            var output = mTable.Parse(input);
            Assert.AreEqual(2, (output.Payload as Expr)?.Eval());
        }
        
        [Test]
        public void ParenthesizedAdd()
        {
            // 1 + (1 + 2) * 3 + 5
            var input = new[]
            {
                Node.Make(new Numeric(1)),
                Node.Make(new OpAdd()),
                Node.Make(new LParen()),
                Node.Make(new Numeric(1)),
                Node.Make(new OpAdd()),
                Node.Make(new Numeric(2)),
                Node.Make(new RParen()),
                Node.Make(new OpMult()),
                Node.Make(new Numeric(3)),
                Node.Make(new OpAdd()),
                Node.Make(new Numeric(5))
            };
            var output = mTable.Parse(input);
            Assert.AreEqual(15, (output.Payload as Expr)?.Eval());
        }
    }
}