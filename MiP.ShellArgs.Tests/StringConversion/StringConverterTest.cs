using System;
using System.ComponentModel;
using System.Globalization;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StringConverter = MiP.ShellArgs.StringConversion.StringConverter;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringConverterTest
    {
        private StringConverter _converter;

        [TestInitialize]
        public void Initialize()
        {
            _converter = new StringConverter(new ParserSettings().ParserProvider);
        }

        [TestMethod]
        public void CanConvertToInt32()
        {
            object result = _converter.To(typeof (int), "123");
            result.Should().Be(123);
        }

        [TestMethod]
        public void CanConvertToNullableInt32()
        {
            object result = _converter.To(typeof (int?), "123");
            result.Should().Be(123);
        }

        //[TestMethod]
        public void CanConvertToDateTimeWithFormat()
        {
            // TODO: after implement format/pattern reactivate this test
            object result = _converter.To(typeof (DateTime), "08 09 10");
            result.Should().Be(new DateTime(2009,08,10));
        }

        [TestMethod]
        public void CanConvertToDateTimeWithoutFormat()
        {
            object result = _converter.To(typeof (DateTime), "2010-12-24");
            result.Should().Be(new DateTime(2010, 12, 24));
        }

        [TestMethod]
        public void CanConvertToNullableDateTime()
        {
            object result = _converter.To(typeof (DateTime?), "2009-08-10");
            result.Should().Be(new DateTime(2009, 08, 10));
        }

        [TestMethod]
        public void CanConvertToEnum()
        {
            object result = _converter.To(typeof (TestEnum), "Zwei");
            result.Should().Be(TestEnum.Zwei);
        }

        [TestMethod]
        public void CanConvertToNullableEnum()
        {
            object result = _converter.To(typeof (TestEnum?), "Drei");
            result.Should().Be(TestEnum.Drei);
        }

        [TestMethod]
        public void CanConvertToTimeSpan()
        {
            object result = _converter.To(typeof (TimeSpan), "1.2:03:04");

            result.Should().Be(new TimeSpan(1,2,3,4));
        }

        [TestMethod]
        public void CanConvertToTimeSpanWithFormat()
        {
            object result = _converter.To(typeof (TimeSpan), "2.12:23:34");

            result.Should().Be(new TimeSpan(2, 12, 23, 34));
        }

        [TestMethod]
        public void ConvertWithTypeDescriptor()
        {
            object result = _converter.To(typeof (CustomTypeForTypeDescriptorTest), "Hello World");

            result.Should().NotBeNull();
            result.GetType().Should().Be(typeof (CustomTypeForTypeDescriptorTest));

            var custom = (CustomTypeForTypeDescriptorTest)result;
            custom.Value.Should().Be("Hello World");

            CustomTypeConverter.LastConvertedValue.Should().Be("Hello World");
        }

        [TypeConverter(typeof (CustomTypeConverter))]
        public class CustomTypeForTypeDescriptorTest
        {
            public string Value;
        }

        public class CustomTypeConverter : TypeConverter
        {
            public static string LastConvertedValue;

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof (string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                LastConvertedValue = (string)value;

                return new CustomTypeForTypeDescriptorTest {Value = (string)value};
            }
        }

        private enum TestEnum
        {
            Null,
            Eins,
            Zwei,
            Drei
        }
    }
}