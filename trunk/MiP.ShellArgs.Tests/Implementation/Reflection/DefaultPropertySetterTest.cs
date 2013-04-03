using System.Reflection;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation.Reflection
{
    [TestClass]
    public class DefaultPropertySetterTest
    {
        private TestProperties _instance;
        private DefaultPropertySetter _valueSetter;
        private DefaultPropertySetter _nullableValueSetter;

        [TestInitialize]
        public void Initialize()
        {
            _instance = new TestProperties();

            PropertyInfo valuePropertyInfo = typeof (TestProperties).GetProperty("Value");
            _valueSetter = new DefaultPropertySetter(new StringConverter(new ParserSettings().ParserProvider), valuePropertyInfo, _instance);

            PropertyInfo nullableValuePropertyInfo = typeof (TestProperties).GetProperty("NullableValue");
            _nullableValueSetter = new DefaultPropertySetter(new StringConverter(new ParserSettings().ParserProvider), nullableValuePropertyInfo, _instance);
        }

        [TestMethod]
        public void SetsValueOnProperty()
        {
            _valueSetter.SetValue("1");

            Assert.AreEqual(1, _instance.Value);
        }

        [TestMethod]
        public void SetsValueOnNullableProperty()
        {
            _nullableValueSetter.SetValue("1");

            Assert.AreEqual(1, _instance.NullableValue);
        }

        [TestMethod]
        public void CallsOnValueSet()
        {
            ValueSetEventArgs eventArgs = null;

            _nullableValueSetter.ValueSet += (o, e) => eventArgs = e;
            _nullableValueSetter.SetValue("1");

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(1, eventArgs.Value);
        }

        public class TestProperties
        {
            public int Value { get; set; }

            public int? NullableValue { get; set; }
        }
    }
}