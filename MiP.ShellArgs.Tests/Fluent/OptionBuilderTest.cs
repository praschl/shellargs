using System.Linq;

using FakeItEasy;

using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Fluent
{
    [TestClass]
    public class OptionBuilderTest
    {
        private OptionBuilder _builder;
        private OptionDefinition _optionDefinition;
        private OptionContext _optionContext;

        [TestInitialize]
        public void Initialize()
        {
            var parser = A.Fake<IParserBuilder>();
            var stringConverter = A.Fake<IStringConverter>();

            _optionDefinition = new OptionDefinition();
            _optionContext = new OptionContext();

            _builder = new OptionBuilder(parser, _optionDefinition, stringConverter, _optionContext);
        }

        [TestMethod]
        public void SetsName()
        {
            _builder.Named("Something");

            Assert.AreEqual("Something", _optionDefinition.Name);
        }

        [TestMethod]
        public void AddsAliases()
        {
            var expected = new[] {"a", "b", "c"};

            _builder.Alias(expected);

            CollectionAssert.AreEquivalent(expected, _optionDefinition.Aliases.ToArray());
        }

        [TestMethod]
        public void SetsPosition()
        {
            _builder.AtPosition(1);

            Assert.AreEqual(1, _optionDefinition.Position);
            Assert.IsTrue(_optionDefinition.IsPositional);
        }

        [TestMethod]
        public void SetsRequired()
        {
            _builder.Required();

            Assert.IsTrue(_optionDefinition.IsRequired);
        }

        [TestMethod]
        public void SetsCollection()
        {
            _builder = (OptionBuilder)_builder.Collection;

            Assert.IsTrue(_optionDefinition.IsCollection);
        }

        [TestMethod]
        public void SetsDescription()
        {
            _builder.ValueDescription("omg");

            Assert.AreEqual("omg", _optionDefinition.Description.ValueDescription);
        }
    }
}