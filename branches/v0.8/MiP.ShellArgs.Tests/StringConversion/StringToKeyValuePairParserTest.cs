using System;
using System.Collections.Generic;

using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.StringConversion
{
    [TestClass]
    public class StringToKeyValuePairParserTest
    {
        private StringToKeyValuePairParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            var settings = new ParserSettings();

            _parser = new StringToKeyValuePairParser(settings.ParserProvider, settings);
        }

        [TestMethod]
        public void CanParseToReturnsFalse()
        {
            Assert.IsFalse(_parser.CanParseTo(typeof (IDisposable)));
            Assert.IsFalse(_parser.CanParseTo(typeof (List<int>)));
            Assert.IsFalse(_parser.CanParseTo(typeof (List<string>)));
            Assert.IsFalse(_parser.CanParseTo(typeof(KeyValuePair<int, string>)));
        }

        [TestMethod]
        public void CanParseToReturnsTrue()
        {
            Assert.IsTrue(_parser.CanParseTo(typeof (KeyValuePair<string, int>)));
        }

        [TestMethod]
        public void IsValidReturnsFalse()
        {
            Assert.IsFalse(_parser.IsValid(typeof (KeyValuePair<string, string>), "abcde"));
            Assert.IsFalse(_parser.IsValid(typeof (KeyValuePair<string, string>), "=abcde"));
            Assert.IsFalse(_parser.IsValid(typeof (KeyValuePair<string, string>), ":abcde"));
            
            Assert.IsFalse(_parser.IsValid(typeof (KeyValuePair<string, int>), "a=b"));
        }

        [TestMethod]
        public void IsValidReturnsTrue()
        {
            Assert.IsTrue(_parser.IsValid(typeof(KeyValuePair<string, string>), "abc=def"));
            Assert.IsTrue(_parser.IsValid(typeof(KeyValuePair<string, string>), "abc:def"));
            Assert.IsTrue(_parser.IsValid(typeof(KeyValuePair<string, int>), "abc:123"));
        }

        [TestMethod]
        public void ParseReturnsParsedPairOfString()
        {
            var result = _parser.Parse(typeof(KeyValuePair<string, string>), "abc=def");
            Assert.AreEqual(new KeyValuePair<string, string>("abc", "def"), result);
        }
        
        [TestMethod]
        public void ParseReturnsParsedPairOfInt32()
        {
            var result = _parser.Parse(typeof(KeyValuePair<string, int>), "abc=123");
            Assert.AreEqual(new KeyValuePair<string, int>("abc", 123), result);
        }
    }
}