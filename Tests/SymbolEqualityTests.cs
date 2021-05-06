using System;
using NUnit.Framework;
using Sacc;

namespace Tests
{
    public class SymbolEqualityTests
    {
        private static readonly object Obj1 = 1;
        private static readonly object Obj2 = 2.0f;
        private static readonly Type Type1 = typeof(int);
        private static readonly Type Type2 = typeof(float);
        private static readonly Symbol Symbol1 = new(Type1);
        private static readonly Symbol Symbol2 = new(Type2);
        

        [Test(Description = "Symbol shall equal itself")]
        public void EqualsSelf()
        {
            var node1 = new Node(Symbol1, Obj1);
            Assert.AreEqual(node1, node1);
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(node1 == node1);
        }
        
        [Test(Description = "Symbol shall equal one with the same parameters")]
        public void EqualsSameParameters()
        {
            var node1 = new Node(Symbol1, Obj1);
            var node2 = new Node(Symbol1, Obj1);
            Assert.AreEqual(node1, node2);
            Assert.IsTrue(node1 == node2);
        }

        [Test(Description = "Symbol shall equal one with the same static type")]
        public void EqualsSameStaticType()
        {
            var node1 = new Node(Symbol1, Obj1);
            var node2 = new Node(Symbol1, Obj2);
            Assert.AreEqual(node1, node2);
            Assert.IsTrue(node1 == node2);
        }

        [Test(Description = "Symbol shall NOT equal one with a different static type")]
        public void NotEqualDifferentStaticType()
        {
            var node1 = new Node(Symbol1, Obj1);
            var node2 = new Node(Symbol2, Obj1);
            Assert.AreNotEqual(node1, node2);
            Assert.IsFalse(node1 == node2);
        }
        
        [Test(Description = "Symbol shall NOT equal one with completely different parameters")]
        public void NotEqualDifferentParameters()
        {
            var node1 = new Node(Symbol1, Obj1);
            var node2 = new Node(Symbol2, Obj2);
            Assert.AreNotEqual(node1, node2);
            Assert.IsFalse(node1 == node2);
        }
    }
}