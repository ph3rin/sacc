using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sacc
{
    public class ParseTableBuilder
    {
        private struct TableEntry
        {
            public ParseAction Action { get; }
            public Symbol Symbol { get; }
            public int SrcId { get; }
            public int DestId { get; }

            public TableEntry(ParseAction action, Symbol symbol, int srcId, int destId = -1)
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
        private readonly List<TableEntry> mTableEntries = new();

        public void BuildTableForCfg(Cfg cfg)
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
                        // This is necessarily a shift operation

                        Debug.Assert(action.Type == ParseActionType.Shift);

                        var destStateId = -1;

                        if (!mStates.Add(destState))
                        {
                            mStates.TryGetValue(destState, out destState);
                            destStateId = mStates2Id[destState];
                        }
                        else
                        {
                            queue.Enqueue(destState);
                            destStateId = RegisterNewState(destState);
                        }

                        mTableEntries.Add(new TableEntry(action, symbol, srcStateId, destStateId));
                    }
                    else
                    {
                        mTableEntries.Add(new TableEntry(action, symbol, srcStateId));
                    }
                }
            }
        }

        private int RegisterNewState(ParserState state)
        {
            var ret = mId2States.Count;
            mStates2Id.Add(state, mId2States.Count);
            mId2States.Add(state);
            return ret;
        }

        public string Dump()
        {
            var builder = new StringBuilder();
            var output = new List<(int, string)>();
            var indices = Enumerable.Range(0, mId2States.Count).ToList();
            var id2String = mId2States
                .Select(state => state.AllItems)
                .Select(items => 
                    string.Join("\n", 
                        items.Select(item => item.ToString()).OrderBy(_ => _, StringComparer.Ordinal)))
                .ToArray();
            
            indices.Sort((lhs, rhs) => string.Compare(id2String[lhs], id2String[rhs], StringComparison.Ordinal));
            var id2PrintedId = new int[indices.Count];
            for (var i = 0; i < indices.Count; ++i)
            {
                id2PrintedId[indices[i]] = i;
            }

            foreach (var entry in mTableEntries)
            {
                var srcPrintId = id2PrintedId[entry.SrcId];
                
                switch (entry.Action.Type)
                {
                    case ParseActionType.Shift:
                    {
                        output.Add((srcPrintId,
                            $"{srcPrintId} => {entry.Symbol}: {id2PrintedId[entry.DestId]} SHIFT"));
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
            
            output.Sort();
            builder.AppendJoin('\n', output.Select(pair => pair.Item2));
            builder.AppendLine();
            
            for (var printId = 0; printId < indices.Count; ++printId)
            {
                var actualId = indices[printId];
                builder.AppendLine($"\n======== State {printId} =========");
                builder.AppendLine(id2String[actualId]);
            }

            return builder.ToString();
        }
    }
}