using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;

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
            Action mapToContainer = () => _converter.MapToContainer(new[] {Token.CreateOption("xyz")}, _context);

            mapToContainer.ShouldThrow<ParsingException>().WithMessage("'xyz' is not a valid option.");
        }

        [TestMethod]
        public void ThrowsWhenValueBeforeOption()
        {
            Action mapToContainer = () => _converter.MapToContainer(new[] {Token.CreateValue("xyz")}, _context);

            mapToContainer.ShouldThrow<ParsingException>().WithMessage("Expected an option instead of value 'xyz'.");
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

            values1.ShouldAllBeEquivalentTo(expected1);
            values2.ShouldAllBeEquivalentTo(expected2);
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

            Action mapToContainer = () => _converter.MapToContainer(tokens, _context);

            mapToContainer.ShouldThrow<ParsingException>()
                .WithMessage("The following option(s) are required, but were not given: [set].");
        }
    }
}