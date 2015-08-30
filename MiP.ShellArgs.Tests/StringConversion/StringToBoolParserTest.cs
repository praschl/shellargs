using FluentAssertions;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringToBoolParserTest
    {
        private StringToBoolParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _parser = new StringToBoolParser();
        }

        [TestMethod]
        public void ReturnsCorrectValue()
        {
            _parser.Parse(typeof (bool), "true").Should().Be(true);
            _parser.Parse(typeof(bool), "false").Should().Be(false);
            
            _parser.Parse(typeof(bool), "True").Should().Be(true);
            _parser.Parse(typeof(bool), "False").Should().Be(false);

            _parser.Parse(typeof(bool), "+").Should().Be(true);
            _parser.Parse(typeof(bool), "-").Should().Be(false);
        }
    }
}