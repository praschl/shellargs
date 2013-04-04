using System;
using System.Collections.Generic;

using MiP.ShellArgs.Implementation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class TypeExtensionsTest
    {
        [TestMethod]
        public void MakeNotNullableReturnsInnerType()
        {
            Type type = typeof (int?);

            Assert.AreEqual(typeof (int), type.MakeNotNullable());
        }

        [TestMethod]
        public void MakeNotNullableReturnsOriginalType()
        {
            Type type = typeof (int);

            Assert.AreEqual(type, type.MakeNotNullable());
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsTrueForList()
        {
            Type type = typeof (List<int>);

            Assert.IsTrue(type.IsOrImplementsICollection());
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsTrueForICollection()
        {
            Type type = typeof (ICollection<int>);

            Assert.IsTrue(type.IsOrImplementsICollection());
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsFalse()
        {
            Type type = typeof(IDisposable);

            Assert.IsFalse(type.IsOrImplementsICollection());
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsFalseForString()
        {
            Type type = typeof(string);

            Assert.IsFalse(type.IsOrImplementsICollection());
        }

        [TestMethod]
        public void GetCollectionItemTypeReturnsItemType()
        {
            Type type = typeof (List<int?>);

            Assert.AreEqual(typeof (int?), type.GetCollectionItemType());
        }

        [TestMethod]
        public void GetCollectionItemTypeReturnsOriginalType()
        {
            Type type = typeof(int?);

            Assert.AreEqual(typeof(int?), type.GetCollectionItemType());
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsTrueForDictionary()
        {
            Type type = typeof (IDictionary<string, int>);

            Assert.IsTrue(type.IsOrImplementsICollection());
        }

        [TestMethod]
        public void GetCollectionItemTypeReturnsKeyValuePairForDictionary()
        {
            Type type = typeof (Dictionary<string, int>);

            Assert.AreEqual(typeof (KeyValuePair<string, int>), type.GetCollectionItemType());
        }
    }
}