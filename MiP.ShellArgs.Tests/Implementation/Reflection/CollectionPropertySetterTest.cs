using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;
using MiP.ShellArgs.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation.Reflection
{
    [TestClass]
    public class CollectionPropertySetterTest
    {
        private CollectionProperties _instance;
        private List<int> _expected;
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

            _expected = new List<int>
                        {
                            1
                        };
        }

        [TestMethod]
        public void AppendsValueToList()
        {
            _instance.List = new List<int>();

            var listSetter = new CollectionPropertySetter(_stringConverter, _listPropertyInfo, _instance);

            listSetter.SetValue("1");
            listSetter.SetValue("2");
            listSetter.SetValue("3");

            _expected.Add(2);
            _expected.Add(3);

            CollectionAssert.AreEqual(_expected, _instance.List);
        }

        [TestMethod]
        public void ChangingCollectionInstanceAfterInitOfCollectionPropertySetterIsNotSupported()
        {
            var listSetter = new CollectionPropertySetter(_stringConverter, _listPropertyInfo, _instance);

            _instance.List = new List<int>();

            listSetter.SetValue("1");
            listSetter.SetValue("2");
            listSetter.SetValue("3");

            Assert.AreEqual(0, _instance.List.Count);
        }

        [TestMethod]
        public void ThrowsWhenReadOnlyPropertyIsNotInitialized()
        {
            string expectedMessage = string.Format("The read only collection property '{0}' is not initialized.", _readonlyPropertyInfo);
            ExceptionAssert.Throws<ParserInitializationException>(() => new CollectionPropertySetter(_stringConverter, _readonlyPropertyInfo, _readonlyInstance),
                ex => Assert.AreEqual(expectedMessage, ex.Message));
        }

        [TestMethod]
        public void CanInitializeConcreteProperty()
        {
            var listSetter = new CollectionPropertySetter(_stringConverter, _listPropertyInfo, _instance);

            listSetter.SetValue("1");

            CollectionAssert.AreEqual(_expected, _instance.List);
        }

        [TestMethod]
        public void CanInitializeICollectionProperty()
        {
            var icollectionSetter = new CollectionPropertySetter(_stringConverter, _icollectionPropertyInfo, _instance);

            icollectionSetter.SetValue("1");

            Assert.IsNotNull(_instance.ICollection);
            Assert.AreEqual(1, _instance.ICollection.Count);
            Assert.AreEqual(1, _instance.ICollection.First());
        }

        [TestMethod]
        public void CallsOnValueSet()
        {
            var icollectionSetter = new CollectionPropertySetter(_stringConverter, _icollectionPropertyInfo, _instance);

            ValueSetEventArgs eventArgs = null;

            icollectionSetter.ValueSet += (o, e) => eventArgs = e;
            icollectionSetter.SetValue("1");

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(1, eventArgs.Value);
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