namespace Sacc
{
    public struct Transition
    {
        public ParseAction Action { get; }
        public Item Src { get; }
        public Item? Dest { get; }

        public Transition(ParseAction action, Item src, Item? dest = null)
        {
            Action = action;
            Src = src;
            Dest = dest;
        }

        private bool Equals(Transition other)
        {
            return Action.Equals(other.Action) && Equals(Src, other.Src) && Equals(Dest, other.Dest);
        }

        public override bool Equals(object? obj)
        {
            return obj is Transition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Action.GetHashCode();
                hashCode = (hashCode * 397) ^ (Src != null ? Src.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Dest != null ? Dest.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Transition lhs, Transition rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Transition lhs, Transition rhs)
        {
            return !(lhs == rhs);
        }
    }
}