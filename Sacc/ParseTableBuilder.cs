using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Diagnostics;

namespace Sacc
{
    public class ParseTableBuilder
    {
        private readonly HashSet<ParserState> mStates = new HashSet<ParserState>();
    
        public void BuildTableForCfg(Cfg cfg)
        {
            var initial = cfg.MakeInitialParserState();
            mStates.Add(initial);
            var queue = new Queue<ParserState>(mStates);
            while (queue.Count > 0)
            {
                var srcState = queue.Dequeue();
                foreach (var symbol in cfg.AllSymbols)
                {
                    var (destState, action) = srcState.TransitionedOn(symbol);
                    if (destState is null) continue;

                    if (!mStates.Add(destState))
                    {
                        mStates.TryGetValue(destState, out destState);
                    }
                    else
                    {
                        queue.Enqueue(destState);
                    }                    
                }
            }
        }
    }
}