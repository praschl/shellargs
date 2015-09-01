using FluentAssertions;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringToObjectParserTest
    {
        private StringToObjectParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _parser = new StringToObjectParser();
        }

        [TestMethod]
        public void ParsesEnums()
        {
            var result = (Numbers)_parser.Parse(typeof (Numbers), "One");
            result.Should().Be(Numbers.One);
        }

        [TestMethod]
        public void ParsesInt()
        {
            var result = (int)_parser.Parse(typeof (int), "1234");
            result.Should().Be(1234);
        }

        [TestMethod]
        public void CallsIsValid()
        {
            _parser.IsValid(typeof (int), "abc").Should().BeFalse();
            _parser.IsValid(typeof (int), "123").Should().BeTrue();
        }

        private enum Numbers
        {
            One = 1
        }
    }
}