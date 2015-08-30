using System;
using System.Collections.Generic;

using FluentAssertions;

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
            _parser.CanParseTo(typeof (IDisposable)).Should().BeFalse();
            _parser.CanParseTo(typeof (List<int>)).Should().BeFalse();
            _parser.CanParseTo(typeof (List<string>)).Should().BeFalse();
            _parser.CanParseTo(typeof(KeyValuePair<int, string>)).Should().BeFalse();
        }

        [TestMethod]
        public void CanParseToReturnsTrue()
        {
            _parser.CanParseTo(typeof (KeyValuePair<string, int>)).Should().BeTrue();
        }

        [TestMethod]
        public void IsValidReturnsFalse()
        {
            _parser.IsValid(typeof (KeyValuePair<string, string>), "abcde").Should().BeFalse();
            _parser.IsValid(typeof (KeyValuePair<string, string>), "=abcde").Should().BeFalse();
            _parser.IsValid(typeof (KeyValuePair<string, string>), ":abcde").Should().BeFalse();
            
            _parser.IsValid(typeof (KeyValuePair<string, int>), "a=b").Should().BeFalse();
        }

        [TestMethod]
        public void IsValidReturnsTrue()
        {
            _parser.IsValid(typeof(KeyValuePair<string, string>), "abc=def").Should().BeTrue();
            _parser.IsValid(typeof(KeyValuePair<string, string>), "abc:def").Should().BeTrue();
            _parser.IsValid(typeof(KeyValuePair<string, int>), "abc:123").Should().BeTrue();
        }

        [TestMethod]
        public void ParseReturnsParsedPairOfString()
        {
            var result = _parser.Parse(typeof(KeyValuePair<string, string>), "abc=def");
            result.ShouldBeEquivalentTo(new KeyValuePair<string, string>("abc", "def"));
        }
        
        [TestMethod]
        public void ParseReturnsParsedPairOfInt32()
        {
            var result = _parser.Parse(typeof(KeyValuePair<string, int>), "abc=123");
            result.ShouldBeEquivalentTo(new KeyValuePair<string, int>("abc", 123));
        }
    }
}