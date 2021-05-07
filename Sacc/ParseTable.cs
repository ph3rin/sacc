using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sacc
{
    public class ParseTable
    {
        public class Entry
        {
            public ParseAction Action { get; }
            private readonly int? mDestStateId;

            public Entry(ParseAction action, int? destStateId = null)
            {
                Action = action;
                mDestStateId = destStateId;
            }

            public int Dest => mDestStateId ?? throw new InvalidOperationException(
                "Entry does not point to a goto state");
        }

        private readonly Dictionary<Symbol, Entry>[] mTable;

        public ParseTable(Dictionary<Symbol, Entry>[] table)
        {
            mTable = table;
        }

        public Node Parse(Node[] input)
        {
            var parseStack = new Stack<Node>();
            var inputStack = new Stack<Node>(input
                .Append(Node.Make(new EndOfInput()))
                .Reverse());
            var stateStack = new Stack<int>();
            stateStack.Push(0);

            while (true)
            {
                var next = inputStack.Peek();
                var stateId = stateStack.Peek();
                if (mTable[stateId].TryGetValue(next.Symbol, out var entry))
                {
                    var actionType = entry.Action.Type;
                    switch (actionType)
                    {
                        case ParseActionType.Shift:
                        {
                            inputStack.Pop();
                            parseStack.Push(next);
                            stateStack.Push(entry.Dest);
                            break;
                        }
                        case ParseActionType.Reduce:
                        {
                            var production = entry.Action.Production;
                            Debug.Assert(parseStack.Count >= production.Ingredients.Length);
                            var nodes = new Node[production.Ingredients.Length];
                            for (var i = nodes.Length - 1; i >= 0; --i)
                            {
                                nodes[i] = parseStack.Pop();
                                stateStack.Pop();
                            }
                            inputStack.Push(production.Reduce(nodes));
                            break;
                        }
                        case ParseActionType.Accept:
                            Debug.Assert(parseStack.Count == 1);
                            return parseStack.Peek();
                        default:
                            throw new ArgumentException();
                    }
                }
                else
                {
                    throw new Exception("Parser rejects the input");
                }
            }
        }
    }
}