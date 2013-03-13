using System;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringToDateTimeParserTest
    {
        private StringToDateTimeParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _parser = new StringToDateTimeParser();
        }

        [TestMethod]
        public void ReturnsValueAsObject()
        {
            object asObject = ((IStringParser)_parser).Parse("3/8/2013");

            Assert.AreEqual(new DateTime(2013, 3, 8), asObject);
        }

        [TestMethod]
        public void ReturnsCorrectValue()
        {
            Assert.AreEqual(new DateTime(2013, 3, 8), _parser.Parse("3/8/2013"));
        }
    }
}