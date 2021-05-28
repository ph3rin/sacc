using System;
using System.Collections.Generic;
using System.Text;

namespace Sacc
{
    public class Item
    {
        public ProductionRule Production { get; }

        /// <summary>
        /// The index of the next symbol that this Item
        /// is transitioning on. If it is production.length,
        /// then it means the follow symbol is expected.
        /// </summary>
        public int DotPos { get; }

        /// <summary>
        /// The follow symbol of this item.
        /// When the production is satisfied, we can only
        /// do the reduction if the next symbol in the input
        /// is same as the follow symbol.
        /// </summary>
        public Symbol Follow { get; }

        public bool CanReduce => DotPos >= Production.Ingredients.Length;

        public Item(ProductionRule production, int dotPos, Symbol follow)
        {
            Production = production;
            DotPos = dotPos;
            Follow = follow;
        }

        public Item ShiftedByOne()
        {
            if (DotPos >= Production.Ingredients.Length)
                throw new InvalidOperationException(
                    "Item with the dot at the end of production cannot shift more symbols.");

            return new Item(Production, DotPos + 1, Follow);
        }

        private bool Equals(Item other)
        {
            return Production.Equals(other.Production) && DotPos == other.DotPos && Follow.Equals(other.Follow);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Production.GetHashCode();
                hashCode = (hashCode * 397) ^ DotPos;
                hashCode = (hashCode * 401) ^ Follow.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("{0} :==", Production.Product);
            var ingredients = Production.Ingredients;
            for (var i = 0; i < ingredients.Length; ++i)
            {
                builder.Append(' ');
                if (i == DotPos) builder.Append('.');
                builder.Append(ingredients[i]);
            }

            if (DotPos == ingredients.Length)
            {
                builder.Append(" . ");
            }
            
            builder.AppendFormat(", {0}", Follow);
            return builder.ToString();
        }
    }
}