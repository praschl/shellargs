using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.Tests.TestHelpers;

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

            Assert.AreEqual(5, result);
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

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Add);
        }

        [TestMethod]
        public void RegisterOptionAfterAContainerWasParsed()
        {
            var parser = new Parser();
            parser.RegisterContainer<NewContainer>()
                .With(x => x.Add)
                .Do(c1 =>
                    {
                        _i1 = c1.Value;
                        c1.Parser
                            .RegisterOption("sub")
                            .As<int>()
                            .Do(c2 => _i2 = c2.Value);
                    });

            parser.Parse("-add", "2", "-sub", "5");

            Assert.AreEqual(7, _i1 + _i2);
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

            ExceptionAssert.Throws<ParserInitializationException>(() =>
                parser.Parse("-add", "irgendwas"),
                ex => Assert.AreEqual("The following names or aliases are not unique: [add].", ex.Message));
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

            ExceptionAssert.Throws<ParserInitializationException>(() =>
                parser.Parse("-add", "irgendwas"),
                ex => Assert.AreEqual("The following names or aliases are not unique: [add].", ex.Message));
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

            ExceptionAssert.Throws<ParserInitializationException>(() =>
                parser.Parse("-add", "irgendwas"),
                ex => Assert.AreEqual("The following options have no unique position: [add, sub].", ex.Message));
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

            ExceptionAssert.Throws<ParserInitializationException>(() =>
                parser.Parse("-add", "irgendwas"),
                ex => Assert.AreEqual("The following options have no unique position: [add, Sub].", ex.Message));
        }

        [TestMethod]
        public void RegisteredPositionalOptionCanBeUsed()
        {
            _i1 = 0;
            _i2 = 0;
            _i3 = 0;

            Parser parser = CreateParserWithThreeDynamicAddOptions();

            parser.Parse("-add1", "2", "-add2", "3", "-add3", "4");
            Assert.AreEqual(9, _i1 + _i2 + _i3);
        }

        [TestMethod]
        public void RegisteredPositionalOptionCanBeUsedWithoutNames()
        {
            _i1 = 0;
            _i2 = 0;
            _i3 = 0;

            Parser parser = CreateParserWithThreeDynamicAddOptions();

            parser.Parse("2", "3", "4");
            Assert.AreEqual(9, _i1 + _i2 + _i3);
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

            ExceptionAssert.Throws<ParsingException>(() =>
                parser.Parse("-add1", "1"),
                ex => Assert.AreEqual("The following option(s) are required, but were not given: [add2].", ex.Message));
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
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
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