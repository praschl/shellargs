using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;
using MiP.ShellArgs.Tests.TestHelpers;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class TokenConverterTest_MapToContainer
    {
        private TokenConverter _converter;
        private StringConverter _stringConverter;
        private OptionContext _context;

        [TestInitialize]
        public void Initialize()
        {
            _stringConverter = new StringConverter(new ParserSettings().ParserProvider);

            _converter = new TokenConverter(new ArgumentFactory(new ParserSettings()));

            _context = new OptionContext();
        }

        [TestMethod]
        public void ThrowsWhenOptionNotFound()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => _converter.MapToContainer(new[] {Token.CreateOption("xyz")}, _context),
                ex => Assert.AreEqual("'xyz' is not a valid option.", ex.Message));
        }

        [TestMethod]
        public void ThrowsWhenValueBeforeOption()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => _converter.MapToContainer(new[] {Token.CreateValue("xyz")}, _context),
                ex => Assert.AreEqual("Expected an option instead of value 'xyz'.", ex.Message));
        }

        [TestMethod]
        public void CallsSetValue()
        {
            var tokens = new List<Token>
                         {
                             Token.CreateOption("set1"),
                             Token.CreateValue("1"),
                             Token.CreateValue("2"),
                             Token.CreateOption("set2"),
                             Token.CreateValue("3"),
                             Token.CreateValue("4"),
                             Token.CreateOption("set1"),
                             Token.CreateValue("5"),
                         };

            var values1 = new List<int>();
            var values2 = new List<int>();

            _context.Add(new OptionDefinition
                         {
                             Name = "set1",
                             ValueSetter = new DelegatingPropertySetter<int>(_stringConverter, values1.Add)
                         });
            _context.Add(new OptionDefinition
                         {
                             Name = "set2",
                             ValueSetter = new DelegatingPropertySetter<int>(_stringConverter, values2.Add)
                         });

            var expected1 = new List<int>
                            {
                                1,
                                2,
                                5
                            };
            var expected2 = new List<int>
                            {
                                3,
                                4
                            };

            _converter.MapToContainer(tokens, _context);

            CollectionAssert.AreEqual(expected1, values1);
            CollectionAssert.AreEqual(expected2, values2);
        }

        [TestMethod]
        public void ThrowsWhenRequiredOptionNotPassed()
        {
            var tokens = new List<Token>();

            _context.Add(new OptionDefinition
                         {
                             Name = "set",
                             IsRequired = true,
                             ValueSetter = new DelegatingPropertySetter<int>(_stringConverter, x => { })
                         });

            ExceptionAssert.Throws<ParsingException>(
                () => _converter.MapToContainer(tokens, _context),
                ex => Assert.AreEqual("The following option(s) are required, but were not given: [set].", ex.Message));
        }
    }
}