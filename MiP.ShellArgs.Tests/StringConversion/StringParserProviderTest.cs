using System;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringParserProviderTest
    {
        private StringParserProvider _provider;

        [TestInitialize]
        public void Initialize()
        {
            _provider = new StringParserProvider();
        }

        [TestMethod]
        public void DefaultParsersAvailable()
        {
            Assert.IsNotNull(_provider.GetParser(typeof (bool)));
            Assert.IsNotNull(_provider.GetParser(typeof (DateTime)));
            Assert.IsNotNull(_provider.GetParser(typeof (TimeSpan)));
        }

        [TestMethod]
        public void RegisterOverridesParser()
        {
            var expected = new MyParser();

            _provider.RegisterParser(expected);

            IStringParser result = _provider.GetParser(typeof (DateTime));

            Assert.AreSame(expected, result);
        }

        private class MyParser : StringParser<DateTime>
        {
            /// <summary>
            /// Parses the string to &lt;TTarget&gt;
            /// </summary>
            /// <param name="value">The string to parse to &lt;TTarget&gt;.</param>
            /// <returns>An instance of &lt;TTarget&gt; which was parsed from <paramref name="value"/>.</returns>
            public override DateTime Parse(string value)
            {
                return DateTime.Now;
            }
        }
    }
}