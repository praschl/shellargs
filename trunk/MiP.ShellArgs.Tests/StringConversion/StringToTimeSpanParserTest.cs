using System;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringToTimeSpanParserTest
    {
        private StringToTimeSpanParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _parser = new StringToTimeSpanParser();
        }

        [TestMethod]
        public void ReturnsValueAsObject()
        {
            object asObject = ((IStringParser)_parser).Parse("5.4:03:02.1");

            Assert.AreEqual(new TimeSpan(5, 4, 3, 2, 100), asObject);
        }

        [TestMethod]
        public void ReturnsCorrectValue()
        {
            Assert.AreEqual(new TimeSpan(5, 4, 3, 2, 100), _parser.Parse("5.4:03:02.1"));
        }
    }
}