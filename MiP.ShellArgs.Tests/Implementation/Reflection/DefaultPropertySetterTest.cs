using System.Reflection;

using FluentAssertions;

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

            _instance.Value.Should().Be(1);
        }

        [TestMethod]
        public void SetsValueOnNullableProperty()
        {
            _nullableValueSetter.SetValue("1");

            _instance.NullableValue.Should().Be(1);
        }

        [TestMethod]
        public void CallsOnValueSet()
        {
            ValueSetEventArgs eventArgs = null;

            _nullableValueSetter.ValueSet += (o, e) => eventArgs = e;
            _nullableValueSetter.SetValue("1");

            eventArgs.Should().NotBeNull();
            eventArgs.Value.Should().Be(1);
        }

        public class TestProperties
        {
            public int Value { get; set; }

            public int? NullableValue { get; set; }
        }
    }
}