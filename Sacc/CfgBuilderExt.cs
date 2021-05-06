namespace Sacc
{
    public static class CfgBuilderExt
    {
        public static CfgBuilder AddAllProductionsInClass<T>(this CfgBuilder cfgBuilder)
        {
            foreach (var production in ProductionRule.LoadAllInClass<T>())
            {
                cfgBuilder.AddProduction(production);
            }
            return cfgBuilder;
        }
    }
}