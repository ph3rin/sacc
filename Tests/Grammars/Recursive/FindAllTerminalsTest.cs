using NUnit.Framework;
using Sacc;

namespace Tests.Grammars.Recursive
{
    public class FindAllTerminalsTest
    {
        [Test]
        public void FindAllTerminals()
        {
            var cfgCore = new CfgBuilder();
            cfgCore.AddAllProductionsInClass<R>();
            Assert.IsEmpty(cfgCore.FindAllTerminals());
        }
    }
}
