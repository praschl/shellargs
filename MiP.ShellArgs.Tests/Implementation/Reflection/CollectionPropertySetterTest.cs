using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentAssertions;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation.Reflection
{
    [TestClass]
    public class CollectionPropertySetterTest
    {
        private CollectionProperties _instance;
        private ReadonlyCollectionProperties _readonlyInstance;
        private PropertyInfo _readonlyPropertyInfo;
        private StringConverter _stringConverter;
        private PropertyInfo _listPropertyInfo;
        private PropertyInfo _icollectionPropertyInfo;

        [TestInitialize]
        public void Initialize()
        {
            _instance = new CollectionProperties();
            _readonlyInstance = new ReadonlyCollectionProperties();

            _listPropertyInfo = typeof (CollectionProperties).GetProperty("List");
            _icollectionPropertyInfo = typeof (CollectionProperties).GetProperty("ICollection");
            _readonlyPropertyInfo = typeof (ReadonlyCollectionProperties).GetProperty("ReadOnly");

            _stringConverter = new StringConverter(new ParserSettings().ParserProvider);
        }

        [TestMethod]
        public void AppendsValueToList()
        {
            _instance.List = new List<int>();

            var listSetter = new CollectionPropertySetter(_stringConverter, _listPropertyInfo, _instance);

            listSetter.SetValue("1");
            listSetter.SetValue("2");
            listSetter.SetValue("3");

            _instance.List.ShouldAllBeEquivalentTo(new[] {"1", "2", "3"}, o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void ChangingCollectionInstanceAfterInitOfCollectionPropertySetterIsNotSupported()
        {
            var listSetter = new CollectionPropertySetter(_stringConverter, _listPropertyInfo, _instance);

            _instance.List = new List<int>();

            listSetter.SetValue("1");
            listSetter.SetValue("2");
            listSetter.SetValue("3");

            _instance.List.Should().BeEmpty();
        }

        [TestMethod]
        public void ThrowsWhenReadOnlyPropertyIsNotInitialized()
        {
            Action constructor = () => new CollectionPropertySetter(_stringConverter, _readonlyPropertyInfo, _readonlyInstance);

            string expectedMessage = $"The read only collection property '{_readonlyPropertyInfo}' is not initialized.";

            constructor.ShouldThrow<ParserInitializationException>()
                .Which.Message.Should().Be(expectedMessage);
        }

        [TestMethod]
        public void CanInitializeConcreteProperty()
        {
            var listSetter = new CollectionPropertySetter(_stringConverter, _listPropertyInfo, _instance);

            listSetter.SetValue("1");

            _instance.List.ShouldBeEquivalentTo(new [] {1});
        }

        [TestMethod]
        public void CanInitializeICollectionProperty()
        {
            var icollectionSetter = new CollectionPropertySetter(_stringConverter, _icollectionPropertyInfo, _instance);

            icollectionSetter.SetValue("1");

            _instance.ICollection.ShouldAllBeEquivalentTo(new[] {1});
        }

        [TestMethod]
        public void CallsOnValueSet()
        {
            var icollectionSetter = new CollectionPropertySetter(_stringConverter, _icollectionPropertyInfo, _instance);

            ValueSetEventArgs eventArgs = null;

            icollectionSetter.ValueSet += (o, e) => eventArgs = e;
            icollectionSetter.SetValue("1");

            eventArgs.Should().NotBeNull();
            eventArgs.Value.Should().Be(1);
        }

        private class CollectionProperties
        {
            public List<int> List { get; set; }

            public ICollection<int> ICollection { get; set; }
        }

        private class ReadonlyCollectionProperties
        {
#pragma warning disable 649
            private List<int> _readonly;
#pragma warning restore 649
            public List<int> ReadOnly { get { return _readonly; } }
        }
    }
}