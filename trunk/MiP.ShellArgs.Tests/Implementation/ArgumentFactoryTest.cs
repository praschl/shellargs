using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class ArgumentFactoryTest
    {
        private ArgumentFactory _factory;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new ArgumentFactory(new ParserSettings());
        }

        [TestMethod]
        public void NoArgumentGiven()
        {
            const string arg = "thisIs,Just: a value|=withNoArgument";
            Argument argument = _factory.Parse(arg);

            Assert.AreEqual(string.Empty, argument.Name);
            Assert.AreEqual(arg, argument.Value);
        }

        [TestMethod]
        public void ArgumentWithoutValue()
        {
            const string arg = "-ThisIsMyArgument";
            Argument argument = _factory.Parse(arg);

            Assert.IsTrue(string.IsNullOrEmpty(argument.Value));
            Assert.AreEqual(arg.Substring(1), argument.Name);
        }

        [TestMethod]
        public void MultiplePrefixesAreNotBeRemoved()
        {
            const string arg = "-/-ThisIsMyArgumentWithMultiPrefixes";
            Argument argument = _factory.Parse(arg);

            Assert.IsTrue(string.IsNullOrEmpty(argument.Value));
            Assert.AreEqual(arg.Substring(1), argument.Name);
        }

        [TestMethod]
        public void WhenAssignmentGivenNameIsParsed()
        {
            const string arg = "-name:value";
            Argument argument = _factory.Parse(arg);

            Assert.AreEqual("name", argument.Name);
        }

        [TestMethod]
        public void AssignmentWithEmptyNameIsTreatedAsValue()
        {
            const string arg = "-:value";
            Argument argument = _factory.Parse(arg);

            Assert.IsFalse(argument.HasName);
            Assert.IsTrue(argument.HasValue);
            Assert.AreEqual(arg, argument.Value);
        }

        [TestMethod]
        public void AcceptStandAloneOptionPrefixAsValue()
        {
            const string arg = "/";

            Argument argument = _factory.Parse(arg);
            Assert.IsFalse(argument.HasName);
            Assert.IsTrue(argument.HasValue);
            Assert.AreEqual("/", argument.Value);
        }

        [TestMethod]
        public void WhenAssignmentGivenValueIsParsed()
        {
            const string arg = "-name:value";
            Argument argument = _factory.Parse(arg);

            Assert.AreEqual("value", argument.Value);
        }

        [TestMethod]
        public void EmptyAssignmentAreParsedToStringEmpty()
        {
            const string arg = "-name=";
            Argument argument = _factory.Parse(arg);

            Assert.AreEqual(string.Empty, argument.Value);
        }

        [TestMethod]
        public void SimpleBooleanAreParsed()
        {
            const string arg = "-name+";
            Argument argument = _factory.Parse(arg);

            Assert.AreEqual("name", argument.Name);
            Assert.AreEqual("+", argument.Value);
        }

        [TestMethod]
        public void ValueWithSimpleBooleanStayIntact()
        {
            const string arg = "-name=value+";
            Argument argument = _factory.Parse(arg);

            Assert.AreEqual("value+", argument.Value);
        }
    }
}