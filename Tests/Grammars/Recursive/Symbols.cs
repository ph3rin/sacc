using Sacc;

namespace Tests.Grammars.Recursive
{
    public class R
    {
        [Production]
        public R Evil(R r)
        {
            return r;
        }
    }
}