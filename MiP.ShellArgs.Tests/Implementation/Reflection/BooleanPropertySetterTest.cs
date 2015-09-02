using System.Reflection;

using FluentAssertions;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation.Reflection
{
    [TestClass]
    public class BooleanPropertySetterTest
    {
        private BooleanProperties _instance;
        private BooleanPropertySetter _valueSetter;
        private BooleanPropertySetter _nullableValueSetter;

        [TestInitialize]
        public void Initialize()
        {
            _instance = new BooleanProperties();

            PropertyInfo valuePropertyInfo = typeof (BooleanProperties).GetProperty("Value");
            _valueSetter = new BooleanPropertySetter(new StringConverter(new ParserSettings().ParserProvider), valuePropertyInfo, _instance);

            PropertyInfo nullableValuePropertyInfo = typeof (BooleanProperties).GetProperty("NullableValue");
            _nullableValueSetter = new BooleanPropertySetter(new StringConverter(new ParserSettings().ParserProvider), nullableValuePropertyInfo, _instance);
        }

        [TestMethod]
        public void SetsToTrue()
        {
            _valueSetter.SetValue("true");

            _instance.Value.Should().BeTrue();
        }

        [TestMethod]
        public void SetsToFalse()
        {
            _instance.Value = true;

            _valueSetter.SetValue("False");

            _instance.Value.Should().BeFalse();
        }

        [TestMethod]
        public void TogglesToFalse()
        {
            _instance.Value = true;

            _valueSetter.SetValue(TokenConverter.ToggleBoolean);

            _instance.Value.Should().BeFalse();
        }

        [TestMethod]
        public void TogglesToTrue()
        {
            _instance.Value = false;

            _valueSetter.SetValue(TokenConverter.ToggleBoolean);

            _instance.Value.Should().BeTrue();
        }

        [TestMethod]
        public void SetsNullableToTrue()
        {
            _nullableValueSetter.SetValue("true");

            _instance.NullableValue.Should().BeTrue();
        }

        [TestMethod]
        public void CallsOnValueSet()
        {
            ValueSetEventArgs eventArgs = null;

            _nullableValueSetter.ValueSet += (o, e) => eventArgs = e;
            _nullableValueSetter.SetValue("true");

            eventArgs.Should().NotBeNull();
            eventArgs.Value.Should().Be(true);
        }

        private class BooleanProperties
        {
            public bool Value { get; set; }
            public bool? NullableValue { get; set; }
        }
    }
}