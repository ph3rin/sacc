namespace Sacc
{
    public readonly struct ParseAction
    {
        private readonly ParseActionType mParseActionType;
        private readonly ProductionRule? mProductionUsed;
        public ParseActionType Type => mParseActionType;

        public static ParseAction MakeReduce(ProductionRule productionUsed)
        {
            return new(ParseActionType.Reduce, productionUsed);
        }

        public static ParseAction MakeAccept()
        {
            return new(ParseActionType.Accept);
        }

        public static ParseAction MakeShift()
        {
            return new(ParseActionType.Shift);
        }

        public static ParseAction MakeDiscard()
        {
            return new(ParseActionType.Discard);
        }

        public static ParseAction MakeReject()
        {
            return new(ParseActionType.Reject);
        }

        private ParseAction(ParseActionType parseActionType, ProductionRule? productionUsed = null)
        {
            mParseActionType = parseActionType;
            mProductionUsed = productionUsed;
        }

        private bool Equals(ParseAction other)
        {
            return mParseActionType == other.mParseActionType && Equals(mProductionUsed, other.mProductionUsed);
        }

        public override bool Equals(object? obj)
        {
            return obj is ParseAction other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) mParseActionType * 397) ^ (mProductionUsed != null ? mProductionUsed.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ParseAction lhs, ParseAction rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ParseAction lhs, ParseAction rhs)
        {
            return !(lhs == rhs);
        }
    }
}