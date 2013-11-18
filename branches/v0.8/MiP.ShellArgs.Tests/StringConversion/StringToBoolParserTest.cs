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
            Assert.IsTrue((bool)_parser.Parse(typeof(bool), "true"));
            Assert.IsFalse((bool)_parser.Parse(typeof(bool), "false"));

            Assert.IsTrue((bool)_parser.Parse(typeof(bool), "True"));
            Assert.IsFalse((bool)_parser.Parse(typeof(bool), "False"));

            Assert.IsTrue((bool)_parser.Parse(typeof(bool), "+"));
            Assert.IsFalse((bool)_parser.Parse(typeof(bool), "-"));
        }
    }
}