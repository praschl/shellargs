using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.Tests.TestHelpers;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class AddOptionsOnTheFlyTests
    {
        [TestMethod]
        public void NewOptionCanBeParsedWithoutError()
        {
            int result = 0;

            var parser = new Parser();
            parser.RegisterOption("x",
                commandBuilder => commandBuilder.As<string>().Do(c1 =>
                    c1.Parser.RegisterOption("add",
                        addBuilder => addBuilder.As<int>().Do(c2 => result += c2.Value)
                        )
                    ));

            parser.Parse("-x", "irgendwas", "-add", "5");

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void NewContainerCanBeParsedWithoutError()
        {
            var parser = new Parser();
            parser.RegisterOption("x",
                commandBuilder => commandBuilder.As<string>().Do(c1 =>
                    c1.Parser.RegisterContainer<NewContainer>()
                    ));

            var result = parser.Parse("-x", "irgendwas", "-add", "5").Result<NewContainer>();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Add);
        }


        [TestMethod]
        public void RegisterOptionFailsWhenAlreadyExists()
        {
            var parser = new Parser();
            parser.RegisterOption("add",
                commandBuilder => commandBuilder.As<string>().Do(c1 =>
                    c1.Parser.RegisterOption("add",
                        addBuilder => addBuilder.As<int>().Do(c2 => { })
                        )
                    ));

            ExceptionAssert.Throws<ParserInitializationException>(() =>
                parser.Parse("-add", "irgendwas"),
                ex => Assert.AreEqual("The following names or aliases are not unique: [add].", ex.Message));
        }

        [TestMethod]
        public void RegisterContainerShouldFailsWhenAnOptionAlreadyExists()
        {
            var parser = new Parser();
            parser.RegisterOption("add",
                commandBuilder => commandBuilder.As<string>().Do(c1 =>
                    c1.Parser.RegisterContainer<NewContainer>()
                    ));

            ExceptionAssert.Throws<ParserInitializationException>(() =>
                parser.Parse("-add", "irgendwas").Result<NewContainer>(),
                ex => Assert.AreEqual("The following names or aliases are not unique: [add].", ex.Message));
        }

        // TODO: add tests for remaining validation methods.

        #region Classes used by test

        private class NewContainer
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Add { get; private set; }
        }

        #endregion
    }
}