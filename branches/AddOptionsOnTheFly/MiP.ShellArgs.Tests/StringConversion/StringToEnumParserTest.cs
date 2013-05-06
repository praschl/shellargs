using System.ComponentModel;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringToEnumParserTest
    {
        private StringToEnumParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _parser = new StringToEnumParser();
        }

        [TestMethod]
        public void ParsesEnumsCaseInsensitive()
        {
            var result = (Numbers)_parser.Parse(typeof(Numbers), "one");
            Assert.AreEqual(Numbers.One, result);
        }

        [TestMethod]
        public void IsValidIgnoresCase()
        {
            Assert.IsTrue(_parser.IsValid(typeof (Numbers), "one"));
        }

        [TestMethod]
        public void ParsesEnumWithTypeConverter()
        {
            var result = (Numbers2)_parser.Parse(typeof(Numbers2), "zwei");
            Assert.AreEqual(Numbers2.Two, result);
        }

        private enum Numbers
        {
            One = 1
        }

        [TypeConverter(typeof(Numbers2Converter))]
        private enum Numbers2
        {
            Zero = 0,
            Two = 2
        }

        private class Numbers2Converter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
            {
                if (sourceType == typeof(Numbers2))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override bool IsValid(ITypeDescriptorContext context, object value)
            {
                return ((string)value == "zwei");
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if ((string)value == "zwei")
                    return Numbers2.Two;

                return Numbers2.Zero;
            }
        }
    }
}