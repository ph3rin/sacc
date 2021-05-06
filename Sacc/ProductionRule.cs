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
        private MethodInfo Method { get; }

        private readonly int mHashCode;
        
        public ProductionRule(Symbol product, Symbol[] ingredients, MethodInfo method)
        {
            Product = product;
            Ingredients = ingredients;
            Method = method;
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

            return new ProductionRule(
                new Symbol(retType),
                parameterInfos.Select(ExtractSymbolFromParameterInfo).ToArray(),
                method);
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
            return Product.Equals(other.Product) && Ingredients.SequenceEqual(other.Ingredients);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProductionRule) obj);
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
    }
}