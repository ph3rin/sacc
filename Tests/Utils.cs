using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sacc;

namespace Tests
{
    public static class Utils
    {
        public static bool DictionaryEquals<TKey, TValue>(
            this Dictionary<TKey, HashSet<TValue>> lhs,
            Dictionary<TKey, HashSet<TValue>> rhs)
        {
            return lhs.Count == rhs.Count &&
                   lhs.All(pair => rhs.ContainsKey(pair.Key) && rhs[pair.Key].SetEquals(pair.Value));
        }

        public static Dictionary<Symbol, HashSet<Symbol>> MakeSymbolDictionary(List<Type[]> entries)
        {
            var result = new Dictionary<Symbol, HashSet<Symbol>>();
            foreach (var types in entries)
            {
                if (types.Length == 0) continue;
                var key = types.First();
                var set = new HashSet<Symbol>();
                foreach (var value in types.Skip(1))
                {
                    set.Add(new Symbol(value));
                }

                result.Add(new Symbol(key), set);
            }

            return result;
        }

        public static void AssertMatches(this IEnumerable<Item> items, HashSet<string> expected)
        {
            Assert.IsTrue(expected.SetEquals(items.Select(i => i.ToString())));
        }

        public static void AssertNoConflicts(this HashSet<ParserState> states)
        {
            foreach (var state in states)
            {
                foreach (var other in states)
                {
                    if (ReferenceEquals(state, other)) continue;
                    if (state.AllItems.ToHashSet().SetEquals(other.AllItems))
                    {
                        Assert.Fail("Hashset contains conflict");
                    }
                }
            }
        }
    }
}