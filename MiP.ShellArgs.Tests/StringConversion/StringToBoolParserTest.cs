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
        public void ReturnsValueAsObject()
        {
            var asObject = ((IStringParser)_parser).Parse("true");

            Assert.AreEqual(true, asObject);
        }

        [TestMethod]
        public void ReturnsCorrectValue()
        {
            Assert.IsTrue(_parser.Parse("true"));
            Assert.IsFalse(_parser.Parse("false"));

            Assert.IsTrue(_parser.Parse("+"));
            Assert.IsFalse(_parser.Parse("-"));
        }
    }
}