namespace Sacc
{
    public static class ParserStateExt
    {
        public static (ParserState?, ParseAction) TransitionedOn(this ParserState state, Symbol symbol)
        {
            var action = state.TransitionedOn(symbol, out var nextState);
            return (nextState, action);
        } 
    }
}