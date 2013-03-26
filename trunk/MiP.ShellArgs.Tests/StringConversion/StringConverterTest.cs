using System;
using System.ComponentModel;
using System.Globalization;

using MiP.ShellArgs.StringConversion;

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
            _converter = new StringConverter(new StringParserProvider());
        }

        [TestMethod]
        public void CanConvertToInt32()
        {
            object result = _converter.To(typeof (int), "123");
            Assert.AreEqual(123, result);
        }

        [TestMethod]
        public void CanConvertToNullableInt32()
        {
            object result = _converter.To(typeof (int?), "123");
            Assert.AreEqual(123, result);
        }

        //[TestMethod]
        public void CanConvertToDateTimeWithFormat()
        {
            // TODO: after implement format/pattern reactivate this test
            object result = _converter.To(typeof (DateTime), "08 09 10");
            Assert.AreEqual(new DateTime(2009, 08, 10), result);
        }

        [TestMethod]
        public void CanConvertToDateTimeWithoutFormat()
        {
            object result = _converter.To(typeof (DateTime), "2010-12-24");
            Assert.AreEqual(new DateTime(2010, 12, 24), result);
        }

        [TestMethod]
        public void CanConvertToNullableDateTime()
        {
            object result = _converter.To(typeof (DateTime?), "2009-08-10");
            Assert.AreEqual(new DateTime(2009, 08, 10), result);
        }

        [TestMethod]
        public void CanConvertToEnum()
        {
            object result = _converter.To(typeof (TestEnum), "Zwei");
            Assert.AreEqual(TestEnum.Zwei, result);
        }

        [TestMethod]
        public void CanConvertToNullableEnum()
        {
            object result = _converter.To(typeof (TestEnum?), "Drei");
            Assert.AreEqual(TestEnum.Drei, result);
        }

        [TestMethod]
        public void CanConvertToTimeSpan()
        {
            object result = _converter.To(typeof (TimeSpan), "1.2:03:04");

            Assert.AreEqual(new TimeSpan(1, 2, 3, 4), result);
        }

        [TestMethod]
        public void CanConvertToTimeSpanWithFormat()
        {
            object result = _converter.To(typeof (TimeSpan), "2.12:23:34");

            Assert.AreEqual(new TimeSpan(2, 12, 23, 34), result);
        }

        [TestMethod]
        public void ConvertWithTypeDescriptor()
        {
            object result = _converter.To(typeof (CustomTypeForTypeDescriptorTest), "Hello World");

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof (CustomTypeForTypeDescriptorTest), result.GetType());

            var custom = (CustomTypeForTypeDescriptorTest)result;
            Assert.AreEqual("Hello World", custom.Value);

            Assert.AreEqual("Hello World", CustomTypeConverter.LastConvertedValue);
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