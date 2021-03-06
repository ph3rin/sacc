using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sacc
{
    public class ProductionRule
    {
        public Symbol Product { get; }
        public Symbol[] Ingredients { get; }
        public Symbol? OverridePrecedence { get; }
        private MethodInfo Method { get; }

        private readonly int mHashCode;

        public ProductionRule(Symbol product, Symbol[] ingredients, MethodInfo method,
            Symbol? overridePrecedence = null)
        {
            Product = product;
            Ingredients = ingredients;
            Method = method;
            OverridePrecedence = overridePrecedence;
            mHashCode = CalculateHash();
        }

        public Node Reduce(Node[] nodes)
        {
            ValidateReduction(nodes);
            var payload = Method.Invoke(null, nodes.Select(s => s.Payload).ToArray());
            return new Node(Product, payload);
        }

        private void ValidateReduction(Node[] nodes)
        {
            if (Ingredients.Length != nodes.Length)
            {
                throw new ArgumentException("Count of nodes to reduce does not match the production rule.");
            }

            var typesMatch = Ingredients
                .Zip(nodes, (symbol, node) =>
                {
                    if (node.Payload is null) return true;
                    var nodeType = node.Payload.GetType();
                    var symbolType = symbol.StaticType;
                    return nodeType == symbolType || nodeType.IsSubclassOf(symbolType);
                }).All(b => b);

            if (!typesMatch)
            {
                throw new ArgumentException("Types do not match the rhs of the production rule.");
            }
        }

        /// <summary>
        /// Derive a production rule that matches the given method
        /// </summary>
        /// <param name="method">The method to derive the production rule from </param>
        /// <returns></returns>
        public static ProductionRule Load(MethodInfo method)
        {
            var retType = method.ReturnType;
            var parameterInfos = method.GetParameters();
            var precedenceSymbol = LoadOverridePrecedence(method);
            
            return new ProductionRule(
                new Symbol(retType),
                parameterInfos.Select(ExtractSymbolFromParameterInfo).ToArray(),
                method,
                precedenceSymbol);
        }

        private static Symbol? LoadOverridePrecedence(MethodInfo method)
        {
            if (method.GetCustomAttribute<OverridePrecedenceAttribute>() is { } attribute)
            {
                return attribute.PrecedenceSymbol;
            }

            return null;
        }

        /// <summary>
        /// Derive production rules from all methods in the given type
        /// that has the Production attribute
        /// </summary>
        /// <param name="type">The type which contains the production methods</param>
        /// <returns></returns>
        public static IEnumerable<ProductionRule> LoadAllInClass(Type type)
        {
            return type.GetMethods()
                .Where(m => m.GetCustomAttribute<ProductionAttribute>() != null)
                .Select(Load);
        }

        public static IEnumerable<ProductionRule> LoadAllInClass<T>()
        {
            return LoadAllInClass(typeof(T));
        }

        private static Symbol ExtractSymbolFromParameterInfo(ParameterInfo parameterInfo)
        {
            return new Symbol(parameterInfo.ParameterType);
        }

        private bool Equals(ProductionRule other)
        {
            return Product.Equals(other.Product) && Ingredients.SequenceEqual(other.Ingredients) &&
                   OverridePrecedence.Equals(other.OverridePrecedence);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return obj?.GetType() == this.GetType() && Equals((ProductionRule) obj);
        }

        public override int GetHashCode()
        {
            return mHashCode;
        }

        private int CalculateHash()
        {
            unchecked
            {
                var hc = Ingredients.Length;
                foreach (var i in Ingredients)
                {
                    hc = hc * 314159 + i.GetHashCode();
                }

                hc ^= OverridePrecedence?.GetHashCode() ?? 0;
                return (Product.GetHashCode() * 397) ^ hc;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0} :== ", Product);
            builder.Append(string.Join(" ", Ingredients));
            return builder.ToString();
        }

        public bool Matches(params Symbol[] symbols)
        {
            return symbols.Length == Ingredients.Length + 1
                   && symbols[0] == Product
                   && symbols.Skip(1).SequenceEqual(Ingredients);
        }
    }
}