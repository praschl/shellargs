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
            Assert.AreEqual(Numbers.One, result);
        }

        [TestMethod]
        public void ParsesInt()
        {
            var result = (int)_parser.Parse(typeof (int), "1234");
            Assert.AreEqual(1234, result);
        }

        [TestMethod]
        public void CallsIsValid()
        {
            Assert.IsFalse(_parser.IsValid(typeof (int), "abc"));
            Assert.IsTrue(_parser.IsValid(typeof (int), "123"));
        }

        private enum Numbers
        {
            One = 1
        }
    }
}