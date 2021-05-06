using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sacc
{
    public class ParserState
    {
        private readonly Cfg mCfg;
        private readonly HashSet<Item> mKernel;
        private HashSet<Item>? mAllItems;
        private readonly int mHashCode;

        public ParserState(Cfg cfg, HashSet<Item> kernel)
        {
            mCfg = cfg;
            mKernel = kernel;
            mAllItems = null;
            mHashCode = CalculateHashCode();
        }

        public ParseActionType TransitionedOn(Symbol symbol, out ParserState? targetState)
        {
            var items = Expand();
            var transitions = new HashSet<Transition>();
            foreach (var item in items)
            {
                var (target, action) = mCfg.FindTransitionOfItemOn(item, symbol);
                if (action.Type == ParseActionType.Discard) continue;
                var transition = new Transition(action, item, target);
                transitions.Add(transition);
            }

            // Cannot find any parser state to go to, so reject
            if (transitions.Count == 0)
            {
                targetState = null;
                return ParseActionType.Reject;
            }

            if (transitions.Count == 1)
            {
                var onlyTransition = transitions.First();
                switch (onlyTransition.Action.Type)
                {
                    case ParseActionType.Shift:
                        targetState = new ParserState(mCfg, new HashSet<Item>
                        {
                            onlyTransition.Dest ?? throw new NullReferenceException()
                        });
                        return ParseActionType.Shift;
                    case ParseActionType.Reduce:
                        targetState = null;
                        return ParseActionType.Shift;
                    case ParseActionType.Accept:
                        targetState = null;
                        return ParseActionType.Accept;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(onlyTransition.Action.Type));
                }
            }

            if (transitions.All(t => t.Action.Type == ParseActionType.Shift))
            {
                targetState = new ParserState(
                    mCfg,
                    transitions
                        .Select(t => t.Dest ?? throw new NullReferenceException())
                        .ToHashSet());
                return ParseActionType.Shift;
            }

            throw new Exception("There is a shift-reduce conflict.");
        }

        private HashSet<Item> Expand()
        {
            if (mAllItems is not null) return mAllItems;
            mAllItems = mKernel.ToHashSet();
            mCfg.FindClosure(mAllItems);
            return mAllItems;
        }

        private int CalculateHashCode()
        {
            unchecked
            {
                var sum = 0;
                foreach (var i in mKernel)
                {
                    sum += i.GetHashCode();
                }

                return sum;
            }
        }

        public override int GetHashCode()
        {
            return mHashCode;
        }

        public IEnumerable<Item> Kernel => mKernel;

        public IEnumerable<Item> AllItems
        {
            get
            {
                Expand();
                return mAllItems ?? throw new NullReferenceException();
            }
        }
    }
}