using System;
using System.Collections.Generic;
using System.Linq;

namespace Sacc
{
    public class Cfg
    {
        [SymbolName("__START__")]
        public class ExtendedStartSymbol
        {
            private readonly object mPayload;

            public ExtendedStartSymbol(object payload)
            {
                mPayload = payload;
            }

            public static ProductionRule MakeProductionFromStartSymbol(Symbol symbol)
            {
                return new ProductionRule(
                    Symbol.Of<ExtendedStartSymbol>(),
                    new[] {symbol},
                    typeof(ExtendedStartSymbol).GetMethod(nameof(Reduce)) ?? throw new NullReferenceException()
                );
            }

            public static ExtendedStartSymbol Reduce(object payload)
            {
                return new(payload);
            }
        }

        private readonly Dictionary<Symbol, HashSet<ProductionRule>> mProductions;
        private readonly Dictionary<Symbol, HashSet<Symbol>> mFirstSymbols;
        private readonly HashSet<Symbol> mTerminals;

        private readonly HashSet<Symbol> mAllSymbols;
        private readonly Symbol mStartSymbol;

        /// <summary>
        /// All the symbols in this CFG, excluding the extended starting symbol
        /// </summary>
        public IReadOnlyCollection<Symbol> AllSymbols => mAllSymbols;
        
        public Cfg(Dictionary<Symbol, HashSet<ProductionRule>> productions,
            HashSet<Symbol> allSymbols,
            Dictionary<Symbol, HashSet<Symbol>> firstSymbols,
            HashSet<Symbol> terminals,
            Symbol startSymbol)
        {
            mProductions = productions;
            mAllSymbols = allSymbols;
            mFirstSymbols = firstSymbols;
            mTerminals = terminals;
            mStartSymbol = startSymbol;
        }

        public bool IsTerminal(Symbol symbol)
        {
            return mTerminals.Contains(symbol);
        }

        public HashSet<Symbol> GetFirstSymbolsOf(Symbol symbol)
        {
            return mFirstSymbols[symbol];
        }

        public IEnumerable<Item> FindAllNeighborsOfItem(Item item)
        {
            var dotPos = item.DotPos;
            var productionLen = item.Production.Ingredients.Length;
            var ingredients = item.Production.Ingredients;
            if (dotPos >= productionLen) yield break;
            var nextSymbol = ingredients[dotPos];
            if (IsTerminal(nextSymbol)) yield break;
            if (!mProductions.TryGetValue(nextSymbol, out var productionsOfNextSymbol)) yield break;
            foreach (var productionOfNextSymbol in productionsOfNextSymbol)
            {
                var followSymbols = dotPos + 1 < productionLen
                    ? GetFirstSymbolsOf(ingredients[dotPos + 1]) as IEnumerable<Symbol>
                    : new[] {item.Follow};
                foreach (var followSymbol in followSymbols)
                {
                    yield return new Item(productionOfNextSymbol, 0, followSymbol);
                }
            }
        }

        public override string ToString()
        {
            return string.Join("\n", mProductions.Values.SelectMany(_ => _));
        }

        public HashSet<Item> FindClosure(HashSet<Item> kernel)
        {
            var queue = new Queue<Item>(kernel);

            while (queue.Count > 0)
            {
                var front = queue.Dequeue();
                foreach (var neighbor in FindAllNeighborsOfItem(front))
                {
                    if (!kernel.Add(neighbor)) continue;
                    queue.Enqueue(neighbor);
                }
            }

            return kernel;
        }

        public (Item?, ParseAction) FindTransitionOfItemOn(Item item, Symbol symbol)
        {
            var productionLen = item.Production.Ingredients.Length;

            if (item.DotPos < productionLen && symbol == item.Production.Ingredients[item.DotPos])
            {
                return (item.ShiftedByOne(), ParseAction.MakeShift());
            }

            if (item.DotPos >= productionLen && symbol == item.Follow)
            {
                if (symbol == Symbol.EndOfInput && item.Production.Product == Symbol.ExtendedStartSymbol)
                {
                    return (null, ParseAction.MakeAccept());
                }

                return (null, ParseAction.MakeReduce(item.Production));
            }

            return (null, ParseAction.MakeDiscard());
        }

        public ParserState MakeInitialParserState()
        {
            return new(this, new HashSet<Item>
            {
                new(ExtendedStartSymbol.MakeProductionFromStartSymbol(mStartSymbol), 0, Symbol.EndOfInput)
            });
        }
    }
}