using System;
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

        public ParseAction TransitionedOn(Symbol symbol, out ParserState? targetState)
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

            if (transitions.Count == 0)
            {
                targetState = null;
                return ParseAction.MakeReject();
            }

            if (transitions.Any(t => t.Action.Type == ParseActionType.Accept))
            {
                targetState = null;
                return ParseAction.MakeAccept();
            }
            
            if (transitions.Any(t => t.Action.Type == ParseActionType.Reduce &&
                                     mCfg.AssociativityOf(t.Action.Production) == Associativity.Left))
            {
                transitions.RemoveWhere(t => t.Action.Type != ParseActionType.Reduce);
                if (transitions.Count > 1) throw new Exception("There is a reduce-reduce conflict.");
                targetState = null;
                return ParseAction.MakeReduce(transitions.First().Action.Production);
            }

            if (transitions.All(t => t.Action.Type == ParseActionType.Reduce))
            {
                if (transitions.Count > 1) throw new Exception("There is a reduce-reduce conflict.");
                targetState = null;
                return ParseAction.MakeReduce(transitions.First().Action.Production);
            }
            
            transitions.RemoveWhere(t => t.Action.Type != ParseActionType.Shift);
            
            if (transitions.Count == 0)
            {
                targetState = null;
                return ParseAction.MakeReject();
            }
            
            targetState = new ParserState(
                mCfg,
                transitions
                    .Select(t => t.Dest ?? throw new NullReferenceException())
                    .ToHashSet());
            return ParseAction.MakeShift();
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

        private bool Equals(ParserState other)
        {
            return mHashCode == other.mHashCode && mKernel.SetEquals(other.mKernel);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ParserState) obj);
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