using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.ContainerAttributes;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class AddOptionsOnTheFlyTests
    {
        private int _i1;
        private int _i2;
        private int _i3;

        [TestMethod]
        public void NewOptionCanBeParsedWithoutError()
        {
            int result = 0;

            var parser = new Parser();
            parser.RegisterOption()
                .Named("x")
                .As<string>()
                .Do(c1 => c1.Parser.RegisterOption()
                    .Named("add")
                    .As<int>()
                    .Do(c2 => result += c2.Value)
                );

            parser.Parse("-x", "irgendwas", "-add", "5");

            result.Should().Be(5);
        }

        [TestMethod]
        public void NewContainerCanBeParsedWithoutError()
        {
            var parser = new Parser();
            parser.RegisterOption()
                .Named("x")
                .As<string>()
                .Do(c1 => c1.Parser.RegisterContainer<NewContainer>()
                );

            var result = parser.Parse("-x", "irgendwas", "-add", "5").Result<NewContainer>();

            result.ShouldBeEquivalentTo(new NewContainer(5));
        }

        [TestMethod]
        public void RegisterOptionAfterAContainerWasParsed()
        {
            List<int> values = new List<int>();

            var parser = new Parser();
            parser.RegisterContainer<NewContainer>()
                .With(x => x.Add)
                .Do(c1 =>
                    {
                        values.Add(c1.Value);
                        c1.Parser
                            .RegisterOption("sub")
                            .As<int>()
                            .Do(c2 => values.Add(c2.Value));
                    });

            parser.Parse("-add", "2", "-sub", "5");

            values.ShouldAllBeEquivalentTo(new[] {2, 5}, o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void RegisterOptionFailsWhenAlreadyExists()
        {
            var parser = new Parser();
            parser.RegisterOption()
                .Named("add")
                .As<string>()
                .Do(c1 => c1.Parser.RegisterOption()
                    .Named("add")
                    .As<int>()
                    .Do(c2 => { })
                );

            Action parse = () => parser.Parse("-add", "irgendwas");

            parse.ShouldThrow<ParserInitializationException>()
                .WithMessage("The following names or aliases are not unique: [add].");
        }

        [TestMethod]
        public void RegisterContainerShouldFailsWhenAnOptionAlreadyExists()
        {
            var parser = new Parser();
            parser.RegisterOption()
                .Named("add")
                .As<string>()
                .Do(c1 => c1.Parser.RegisterContainer<NewContainer>()
                );

            Action parse = () => parser.Parse("-add", "irgendwas");

            parse.ShouldThrow<ParserInitializationException>()
                .WithMessage("The following names or aliases are not unique: [add].");
        }

        [TestMethod]
        public void RegisterOptionFailsWhenPositionsNotUnique()
        {
            var parser = new Parser();
            parser.RegisterOption()
                .Named("add")
                .AtPosition(1)
                .As<string>()
                .Do(c1 => c1.Parser.RegisterOption()
                    .Named("sub")
                    .AtPosition(1)
                    .As<int>()
                    .Do(c2 => { })
                );

            Action parse = () => parser.Parse("-add", "irgendwas");

            parse.ShouldThrow<ParserInitializationException>()
                .WithMessage("The following options have no unique position: [add, sub].");
        }

        [TestMethod]
        public void RegisterContainerShouldFailsWhenPositionsNotUnique()
        {
            var parser = new Parser();
            parser.RegisterOption()
                .Named("add")
                .AtPosition(1)
                .As<string>()
                .Do(c1 => c1.Parser.RegisterContainer<Position1Container>()
                );

            Action parse = () => parser.Parse("-add", "irgendwas");

            parse.ShouldThrow<ParserInitializationException>()
                .WithMessage("The following options have no unique position: [add, Sub].");
        }

        [TestMethod]
        public void RegisteredPositionalOptionCanBeUsed()
        {
            _i1 = 0;
            _i2 = 0;
            _i3 = 0;

            Parser parser = CreateParserWithThreeDynamicAddOptions();

            parser.Parse("-add1", "2", "-add2", "3", "-add3", "4");

            _i1.Should().Be(2);
            _i2.Should().Be(3);
            _i3.Should().Be(4);
        }

        [TestMethod]
        public void RegisteredPositionalOptionCanBeUsedWithoutNames()
        {
            _i1 = 0;
            _i2 = 0;
            _i3 = 0;

            Parser parser = CreateParserWithThreeDynamicAddOptions();

            parser.Parse("2", "3", "4");

            _i1.Should().Be(2);
            _i2.Should().Be(3);
            _i3.Should().Be(4);
        }

        [TestMethod]
        public void MissingRequiredOptionsFails()
        {
            var parser = new Parser();

            parser.RegisterOption()
                .Named("Add1")
                .Required()
                .As<int>()
                .Do(c1 => c1.Parser.RegisterOption()
                    .Named("add2")
                    .Required()
                    .As<int>()
                    .Do(c2 => { }));

            Action parse = () => parser.Parse("-add1", "1");

            parse.ShouldThrow<ParsingException>()
                .WithMessage("The following option(s) are required, but were not given: [add2].");
        }

        private Parser CreateParserWithThreeDynamicAddOptions()
        {
            var parser = new Parser();

            parser.RegisterOption("add1")
                .AtPosition(1)
                .As<int>()
                .Do(Add1Action);

            return parser;
        }

        private void Add1Action(ParsingContext<int> context)
        {
            _i1 = context.Value;
            context.Parser
                .RegisterOption("add2")
                .AtPosition(2)
                .As<int>()
                .Do(Add2Action);
        }

        private void Add2Action(ParsingContext<int> context)
        {
            _i2 = context.Value;
            context.Parser
                .RegisterOption("add3")
                .AtPosition(3)
                .As<int>()
                .Do(Add3Action);
        }

        private void Add3Action(ParsingContext<int> context)
        {
            _i3 = context.Value;
        }

        #region Classes used by test

        private class NewContainer
        {
            public NewContainer()
            {
            }

            public NewContainer(int add)
            {
                Add = add;
            }

            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            public int Add { get; private set; }
        }

        private class Position1Container
        {
            [Position(1)]
            public int Sub { get; private set; }
        }

        #endregion
    }
}