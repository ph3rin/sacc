using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sacc
{
    public class ParseTableBuilder
    {
        private readonly struct Rule
        {
            public ParseAction Action { get; }
            public Symbol Symbol { get; }
            public int SrcId { get; }
            public int? DestId { get; }

            public Rule(ParseAction action, Symbol symbol, int srcId, int? destId = null)
            {
                Action = action;
                Symbol = symbol;
                SrcId = srcId;
                DestId = destId;
            }
        }

        private readonly HashSet<ParserState> mStates = new HashSet<ParserState>();
        private readonly Dictionary<ParserState, int> mStates2Id = new();
        private readonly List<ParserState> mId2States = new();
        private readonly List<Rule> mRules = new();
        private readonly bool mUseCanonicalRepresentation;

        /// <summary>
        /// Creates a parse table builder
        /// </summary>
        /// <param name="useCanonicalRepresentation">Set this to true if you want
        /// the internal IDs of parser states to be consistent with the table's
        /// canonical string representation (that is uniform across platforms)
        /// Useful for debug purposes, but comes with some performance overhead.</param>
        public ParseTableBuilder(bool useCanonicalRepresentation = false)
        {
            mUseCanonicalRepresentation = useCanonicalRepresentation;
        }

        public ParseTable BuildTableForCfg(Cfg cfg)
        {
            var initial = cfg.MakeInitialParserState();
            mStates.Add(initial);
            var queue = new Queue<ParserState>(mStates);
            RegisterNewState(initial);

            while (queue.Count > 0)
            {
                var srcState = queue.Dequeue();
                var srcStateId = mStates2Id[srcState];

                foreach (var symbol in cfg.AllSymbols.Append(Symbol.EndOfInput))
                {
                    var (destState, action) = srcState.TransitionedOn(symbol);
                    if (destState is not null)
                    {
                        // This is necessarily a shift operation, since there destState is not null.
                        Debug.Assert(action.Type == ParseActionType.Shift);

                        var destStateId = -1;

                        if (!mStates.Add(destState))
                        {
                            mStates.TryGetValue(destState, out destState);
                            destStateId = mStates2Id[destState ?? throw new NullReferenceException()];
                        }
                        else
                        {
                            queue.Enqueue(destState);
                            destStateId = RegisterNewState(destState);
                        }

                        mRules.Add(new Rule(action, symbol, srcStateId, destStateId));
                    }
                    else
                    {
                        mRules.Add(new Rule(action, symbol, srcStateId));
                    }
                }
            }

            var rules = mRules;

            if (mUseCanonicalRepresentation)
            {
                var canonical = new CanonicalRepresentation(this);
                rules = canonical.TranslateRules(mRules);
            }

            return BuildTable(rules);
        }

        private ParseTable BuildTable(List<Rule> rules)
        {
            var table = new Dictionary<Symbol, ParseTable.Entry>[mStates.Count];
            for (var i = 0; i < table.Length; ++i)
            {
                table[i] = new Dictionary<Symbol, ParseTable.Entry>();
            }

            foreach (var rule in rules)
            {
                table[rule.SrcId][rule.Symbol] = new ParseTable.Entry(rule.Action, rule.DestId);
            }

            return new ParseTable(table);
        }

        private int RegisterNewState(ParserState state)
        {
            var ret = mId2States.Count;
            mStates2Id.Add(state, mId2States.Count);
            mId2States.Add(state);
            return ret;
        }

        private class CanonicalRepresentation
        {
            private readonly ParseTableBuilder mTableBuilder;
            private readonly string[] mId2String;
            private readonly int[] mPrintId2Id;
            private readonly int[] mId2PrintId;

            public CanonicalRepresentation(ParseTableBuilder builder)
            {
                mTableBuilder = builder;
                mId2String = builder.GetStringRepresentationForAllStates();
                mPrintId2Id = Enumerable.Range(1, builder.mId2States.Count - 1).ToArray();
                Array.Sort(mPrintId2Id, (lhs, rhs) =>
                    string.Compare(mId2String[lhs], mId2String[rhs], StringComparison.Ordinal));
                mPrintId2Id = new[] {0}.Concat(mPrintId2Id).ToArray();
                mId2PrintId = InverseMap(mPrintId2Id);
            }

            public List<Rule> TranslateRules(List<Rule> rules)
            {
                return rules.Select(
                        rule => new Rule(rule.Action, rule.Symbol, GetPrintIdOf(rule.SrcId),
                            rule.DestId.HasValue ? GetPrintIdOf(rule.DestId.Value) : (int?) null))
                    .ToList();
            }

            public int GetPrintIdOf(int id)
            {
                return mId2PrintId[id];
            }

            public string GetStringOf(int printId)
            {
                return mId2String[mPrintId2Id[printId]];
            }

            private static int[] InverseMap(int[] map)
            {
                var result = new int[map.Length];
                for (var i = 0; i < map.Length; ++i)
                {
                    result[map[i]] = i;
                }

                return result;
            }
        }

        private string[] GetStringRepresentationForAllStates()
        {
            return mId2States
                .Select(state => state.AllItems)
                .Select(items =>
                    string.Join("\n",
                        items.Select(item => item.ToString()).OrderBy(_ => _, StringComparer.Ordinal)))
                .ToArray();
        }

        public string Dump()
        {
            var builder = new StringBuilder();
            var canonical = new CanonicalRepresentation(this);
            var output = new List<(int, string)>();

            foreach (var entry in mRules)
            {
                var srcPrintId = canonical.GetPrintIdOf(entry.SrcId);

                switch (entry.Action.Type)
                {
                    case ParseActionType.Shift:
                    {
                        output.Add((srcPrintId,
                            $"{srcPrintId} => {entry.Symbol}: {canonical.GetPrintIdOf(entry.DestId ?? throw new NullReferenceException())} SHIFT"));
                        break;
                    }
                    case ParseActionType.Reduce:
                        output.Add(
                            (srcPrintId, $"{srcPrintId} => {entry.Symbol}: REDUCE |> {entry.Action.Production}"));
                        break;
                    case ParseActionType.Accept:
                        output.Add((srcPrintId, $"{srcPrintId} => {entry.Symbol}: ACCEPT"));
                        break;
                    case ParseActionType.Reject:
                        // Need not to print rejection
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            output.Sort((p1, p2) =>
            {
                if (p1.Item1 != p2.Item1) return p1.Item1.CompareTo(p2.Item1);
                return string.Compare(p1.Item2, p2.Item2, StringComparison.Ordinal);
            });

            builder.AppendJoin('\n', output.Select(pair => pair.Item2));
            builder.AppendLine();

            for (var printId = 0; printId < mStates.Count; ++printId)
            {
                builder.AppendLine($"\n======== State {printId} =========");
                builder.AppendLine(canonical.GetStringOf(printId));
            }

            return builder.ToString();
        }
    }
}