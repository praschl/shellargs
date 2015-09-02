using System.Linq;

using FakeItEasy;

using FluentAssertions;

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

            _optionDefinition.Name.Should().Be("Something");
        }

        [TestMethod]
        public void AddsAliases()
        {
            var expected = new[] {"a", "b", "c"};

            _builder.Alias(expected);

            _optionDefinition.Aliases.ShouldAllBeEquivalentTo(expected);
        }

        [TestMethod]
        public void SetsPosition()
        {
            _builder.AtPosition(1);

            _optionDefinition.Position.Should().Be(1);
            _optionDefinition.IsPositional.Should().BeTrue();
        }

        [TestMethod]
        public void SetsRequired()
        {
            _builder.Required();

            _optionDefinition.IsRequired.Should().BeTrue();
        }

        [TestMethod]
        public void SetsCollection()
        {
            _builder = (OptionBuilder)_builder.Collection;

            _optionDefinition.IsCollection.Should().BeTrue();
        }

        [TestMethod]
        public void SetsDescription()
        {
            _builder.ValueDescription("omg");

            _optionDefinition.Description.ValueDescription.Should().Be("omg");
        }
    }
}