using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Sacc;

namespace Tests
{
    public class ProductionRuleLoading
    {
        private class A
        {
            public int Val { get; set; }

            public class B : A
            {
                public B(int val = 0) : base(val)
                {
                }
            }

            public A(int val = 0)
            {
                Val = val;
            }

            [Production]
            public static A Foo(A lhs, A rhs)
            {
                return new A(lhs.Val + rhs.Val);
            }

            [Production]
            public static A Bar(A lhs, B mid, A rhs)
            {
                return new A(lhs.Val * mid.Val * rhs.Val);
            }
        }

        private readonly MethodInfo mFooInfo = typeof(A).GetMethod("Foo");
        private readonly MethodInfo mBarInfo = typeof(A).GetMethod("Bar");

        [Test(Description = "Loads Foo production rule correctly")]
        public void LoadFooProduction()
        {
            var symbol = new Symbol(typeof(A));
            var expected = new ProductionRule(symbol, new[] {symbol, symbol}, mFooInfo);
            var actual = ProductionRule.Load(mFooInfo);

            Assert.AreEqual(expected, actual);
        }

        [Test(Description = "Loads Bar production rule correctly")]
        public void LoadBarProduction()
        {
            var symbolA = new Symbol(typeof(A));
            var symbolB = new Symbol(typeof(A.B));
            var expected = new ProductionRule(symbolA, new[] {symbolA, symbolB, symbolA}, mBarInfo);
            var actual = ProductionRule.Load(mBarInfo);

            Assert.AreEqual(expected, actual);
        }
        
        [Test(Description = "Reduction on Foo works as expected")]
        public void ReductionFoo()
        {
            var lhs = Node.Make(new A(42));
            var rhs = Node.Make(new A(-41));
            var production = ProductionRule.Load(mFooInfo);
            var product = production.Reduce(new[] {lhs, rhs});

            Assert.AreEqual(1, (product.Payload as A).Val);
        }

        [Test(Description = "Reduction on Bar works as expected (with B deriving from A)")]
        public void ReductionBar()
        {
            var lhs = Node.Make(new A(1));
            var mid = Node.Make(new A.B(2));
            var rhs = Node.Make(new A(3));
            var production = ProductionRule.Load(mBarInfo);
            var product = production.Reduce(new[] {lhs, mid, rhs});

            Assert.AreEqual(6, (product.Payload as A).Val);
        }

        [Test(Description = "Loading all production rules from a class works")]
        public void LoadAllProductionsFromClass()
        {
            var expected = new[]
            {
                ProductionRule.Load(mFooInfo),
                ProductionRule.Load(mBarInfo)
            };
            var actual = ProductionRule.LoadAllInClass(typeof(A));
            
            Assert.IsTrue(actual.ToHashSet().SetEquals(expected));
        }
    }
}