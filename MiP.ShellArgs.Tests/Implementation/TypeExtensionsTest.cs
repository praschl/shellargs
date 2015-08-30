using System;
using System.Collections.Generic;

using FluentAssertions;

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

            type.MakeNotNullable().Should().Be(typeof (int));
        }

        [TestMethod]
        public void MakeNotNullableReturnsOriginalType()
        {
            Type type = typeof (int);
            
            type.MakeNotNullable().Should().Be(typeof(int));
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsTrueForList()
        {
            Type type = typeof (List<int>);

            type.IsOrImplementsICollection().Should().BeTrue();
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsTrueForICollection()
        {
            Type type = typeof (ICollection<int>);

            type.IsOrImplementsICollection().Should().BeTrue();
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsFalse()
        {
            Type type = typeof(IDisposable);

            type.IsOrImplementsICollection().Should().BeFalse();
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsFalseForString()
        {
            Type type = typeof(string);

            type.IsOrImplementsICollection().Should().BeFalse();
        }

        [TestMethod]
        public void GetCollectionItemTypeReturnsItemType()
        {
            Type type = typeof (List<int?>);

            type.GetCollectionItemType().Should().Be(typeof(int?));
        }

        [TestMethod]
        public void GetCollectionItemTypeReturnsOriginalType()
        {
            Type type = typeof(int?);

           type.GetCollectionItemType().Should().Be(typeof(int?));
        }

        [TestMethod]
        public void IsOrImplementsICollectionReturnsTrueForDictionary()
        {
            Type type = typeof (IDictionary<string, int>);

            type.IsOrImplementsICollection().Should().Be(true);
        }

        [TestMethod]
        public void GetCollectionItemTypeReturnsKeyValuePairForDictionary()
        {
            Type type = typeof (Dictionary<string, int>);

            type.GetCollectionItemType().Should().Be(typeof(KeyValuePair<string, int>));
        }
    }
}