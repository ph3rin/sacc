using System.Collections.Generic;

namespace Sacc.Helpers
{
    public class FindFirstsHelper
    {
        public readonly Dictionary<Symbol, HashSet<ProductionRule>> productions;
        public readonly HashSet<Symbol> terminals;

        public FindFirstsHelper(Dictionary<Symbol, HashSet<ProductionRule>> productions, HashSet<Symbol> terminals)
        {
            this.productions = productions;
            this.terminals = terminals;
        }

        public Dictionary<Symbol, HashSet<Symbol>> FindFirsts()
        {
            var result = new Dictionary<Symbol, HashSet<Symbol>>();
            foreach (var terminal in terminals)
            {
                result.Add(terminal, new HashSet<Symbol> {terminal});
            }

            foreach (var variable in productions.Keys)
            {
                if (result.ContainsKey(variable)) continue;
                var firsts = new HashSet<Symbol>();
                var queue = new Queue<Symbol>();
                result.Add(variable, firsts);
                firsts.Add(variable);
                queue.Enqueue(variable);
                while (queue.Count > 0)
                {
                    var symbol = queue.Dequeue();
                    if (!productions.TryGetValue(symbol, out var productionsOfSymbol)) continue;
                    foreach (var production in productionsOfSymbol)
                    {
                        if (production.Ingredients.Length == 0) continue;
                        var candidate = production.Ingredients[0];
                        if (firsts.Contains(candidate)) continue;
                        firsts.Add(candidate);
                        queue.Enqueue(candidate);
                    }
                }

                firsts.RemoveWhere(symbol => !terminals.Contains(symbol));
            }

            return result;
        }
    }
}