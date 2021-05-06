using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sacc.Helpers;

namespace Sacc
{
    public class CfgBuilder
    {
        private readonly Dictionary<Symbol, HashSet<ProductionRule>> mProductions = new();
        private Symbol? mStartSymbol;

        public Cfg Build(Symbol? overrideStartSymbol = null)
        {
            // Do this before adding the extended start symbol
            // so that it does not appear in the "AllSymbols"
            // property of a cfg.
            var allSymbols = ListAllSymbols();
            
            var startSymbol = AddProductionForExtendedStartSymbol(overrideStartSymbol);

            return new Cfg(
                mProductions,
                allSymbols,
                FindFirsts(),
                FindAllTerminals(),
                startSymbol);
        }

        private Symbol AddProductionForExtendedStartSymbol(Symbol? overrideStartSymbol)
        {
            if (overrideStartSymbol is { } startSymbol)
            {
                if (!mProductions.ContainsKey(startSymbol))
                {
                    throw new ArgumentException("Override start symbol is not in the language");
                }
            }
            else if (mStartSymbol is not null)
            {
                startSymbol = mStartSymbol.Value;
            }
            else
            {
                throw new ArgumentException("A CFG must contain one and only one start symbol!");
            }

            AddProduction(Cfg.ExtendedStartSymbol.MakeProductionFromStartSymbol(startSymbol));
            return startSymbol;
        }

        public CfgBuilder AddProduction(ProductionRule production)
        {
            var product = production.Product;
            if (!mProductions.TryGetValue(product, out var ruleSet))
            {
                var productType = product.StaticType;
                if (HasStartSymbolAttribute(productType))
                {
                    if (mStartSymbol is null)
                    {
                        mStartSymbol = product;
                    }
                    else
                    {
                        throw new ArgumentException("A CFG can have only one start symbol.");
                    }
                }

                ruleSet = new HashSet<ProductionRule>();
                mProductions.Add(product, ruleSet);
            }

            ruleSet.Add(production);
            return this;
        }

        public HashSet<Symbol> FindAllTerminals()
        {
            return mProductions
                .Values
                .SelectMany(_ => _)
                .Select(p => p.Ingredients)
                .SelectMany(_ => _)
                .Where(symbol => !mProductions.ContainsKey(symbol))
                .ToHashSet();
        }

        private HashSet<Symbol> ListAllSymbols()
        {
            return mProductions
                .Values
                .SelectMany(_ => _)
                .Select(p => p.Ingredients)
                .SelectMany(_ => _)
                .ToHashSet();
        }

        public Dictionary<Symbol, HashSet<Symbol>> FindFirsts()
        {
            return new FindFirstsHelper(mProductions, FindAllTerminals()).FindFirsts();
        }

        private static bool HasStartSymbolAttribute(Type type)
        {
            return type.GetCustomAttribute<StartSymbolAttribute>() != null;
        }
    }
}