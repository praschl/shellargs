using System;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringParserProviderTest
    {
        private IStringParserProvider _provider;

        [TestInitialize]
        public void Initialize()
        {
            _provider = new ParserSettings().ParserProvider;
        }

        [TestMethod]
        public void DefaultParsersAvailable()
        {
            Assert.AreEqual(typeof (StringToBoolParser), _provider.GetParser(typeof (bool)).GetType());
            Assert.AreEqual(typeof (StringToEnumParser), _provider.GetParser(typeof (StringSplitOptions)).GetType());
            Assert.AreEqual(typeof(StringToObjectParser), _provider.GetParser(typeof(TimeSpan)).GetType());
        }

        [TestMethod]
        public void RegisterOverridesParser()
        {
            var expected = new MyParser();

            _provider.RegisterParser(expected);

            IStringParser result = _provider.GetParser(typeof (DateTime));

            Assert.AreSame(expected, result);
        }

        private class MyParser : StringParser
        {
            /// <summary>
            /// Determines whether this instance can parse to the specified target type.
            /// </summary>
            /// <param name="targetType">Type to parse a string to.</param>
            /// <returns>
            ///   <c>true</c> if a string can be parsed to the specified target type; otherwise, <c>false</c>.
            /// </returns>
            public override bool CanParseTo(Type targetType)
            {
                return true;
            }

            /// <summary>
            /// Determines whether the specified value is valid for the target type.
            /// </summary>
            /// <param name="targetType">Type to convert to.</param>
            /// <param name="value">The value to be converted.</param>
            /// <returns>
            ///   <c>true</c> if the specified value is valid for the target type; otherwise, <c>false</c>.
            /// </returns>
            public override bool IsValid(Type targetType, string value)
            {
                return true;
            }

            /// <summary>
            /// Parses the string to &lt;TTarget&gt;
            /// </summary>
            /// <param name="targetType">Type to convert to.</param>
            /// <param name="value">The string to parse to &lt;TTarget&gt;.</param>
            /// <returns>
            /// An instance of &lt;TTarget&gt; which was parsed from <paramref name="value" />.
            /// </returns>
            public override object Parse(Type targetType, string value)
            {
                return DateTime.Now;
            }
        }
    }
}