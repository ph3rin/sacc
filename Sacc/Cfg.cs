using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Dictionary<Symbol, int> mPrecedence;
        private readonly Dictionary<Symbol, Associativity> mAssociativity;

        /// <summary>
        /// All the symbols in this CFG, excluding the extended starting symbol
        /// </summary>
        public IReadOnlyCollection<Symbol> AllSymbols => mAllSymbols;

        public Cfg(Dictionary<Symbol, HashSet<ProductionRule>> productions,
            HashSet<Symbol> allSymbols,
            Dictionary<Symbol, HashSet<Symbol>> firstSymbols,
            HashSet<Symbol> terminals,
            Symbol startSymbol, Dictionary<Symbol, int> precedence, Dictionary<Symbol, Associativity> associativity)
        {
            mProductions = productions;
            mAllSymbols = allSymbols;
            mFirstSymbols = firstSymbols;
            mTerminals = terminals;
            mStartSymbol = startSymbol;
            mPrecedence = precedence;
            mAssociativity = associativity;
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
            var (target, action) = FindTransitionOfItemOnUnchecked(item, symbol);

            return TransitionIsValidForItem(action, item, symbol)
                ? (target, action)
                : (null, ParseAction.MakeDiscard());
        }

        private (Item?, ParseAction) FindTransitionOfItemOnUnchecked(Item item, Symbol symbol)
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

        private bool TransitionIsValidForItem(ParseAction action, Item item, Symbol inputSymbol)
        {
            /*
             * Constraints:
             *  - If the action is to discard, then this is fine.
             *  - If precedence(lhs) < precedence(rhs), this must be a shift
             *  - If precedence(lhs) > precedence(rhs) and dot is at the end, this must be a reduce
             *  - If either has undefined precedence or associativity, then this is ok
             *  - If precedence(rhs) == precedence(lhs),
             *      - If either are left-associative and dot is at the end, this must be a reduce
             *      - Otherwise, this must be a shift
             */

            var lhsIdx = Array.FindLastIndex(item.Production.Ingredients, HasPrecedence);
            var lhs = item.Production.OverridePrecedence ??
                      (lhsIdx != -1 ? item.Production.Ingredients[lhsIdx] : null);
            var rhs = inputSymbol;
            var pLeft = PrecedenceOf(lhs);
            var pRight = PrecedenceOf(rhs);
            var aLeft = AssociativityOf(lhs);
            var aRight = AssociativityOf(rhs);

            if (pLeft < pRight && action.Type != ParseActionType.Shift)
            {
                return false;
            }

            if (pLeft > pRight && item.CanReduce && action.Type != ParseActionType.Reduce)
            {
                return false;
            }

            if (pLeft is null || pRight is null || aLeft == Associativity.Default || aRight == Associativity.Default)
            {
                return true;
            }

            if (pLeft == pRight)
            {
                Debug.Assert(aLeft == aRight);
                if (aLeft == Associativity.Left || aRight == Associativity.Left)
                {
                    if (item.CanReduce && action.Type != ParseActionType.Reduce)
                    {
                        return false;
                    }
                }
                else if (action.Type != ParseActionType.Shift)
                {
                    return false;
                }
            }

            return true;
        }

        public Associativity AssociativityOf(Symbol? symbol)
        {
            if (symbol is null) return Associativity.Default;
            if (mAssociativity.TryGetValue(symbol.Value, out var result)) return result;
            return Associativity.Default;
        }

        public Associativity AssociativityOf(ProductionRule production)
        {
            if (production.OverridePrecedence.HasValue) return AssociativityOf(production.OverridePrecedence);
            for (var i = production.Ingredients.Length - 1; i >= 0; --i)
            {
                var assoc = AssociativityOf(production.Ingredients[i]);
                if (assoc != Associativity.Default)
                {
                    return assoc;
                }
            }
            return Associativity.Default;
        }

        public int? PrecedenceOf(Symbol? symbol)
        {
            if (symbol is null) return null;
            if (mPrecedence.TryGetValue(symbol.Value, out var result)) return result;
            return null;
        }

        private bool HasPrecedence(Symbol symbol)
        {
            return mPrecedence.ContainsKey(symbol);
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