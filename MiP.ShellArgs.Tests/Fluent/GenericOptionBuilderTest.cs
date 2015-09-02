using FakeItEasy;

using FluentAssertions;

using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Fluent
{
    [TestClass]
    public class GenericOptionBuilderTest
    {
        private OptionDefinition _optionDefinition;
        private OptionBuilder<string> _stringOptionBuilder;
        private IParserBuilder _parser;
        private IStringConverter _stringConverter;
        private OptionContext _optionContext;

        [TestInitialize]
        public void Initialize()
        {
            _parser = A.Fake<IParserBuilder>();
            _stringConverter = A.Fake<IStringConverter>();

            _optionDefinition = new OptionDefinition {Name = "notImportant"};
            _optionContext = new OptionContext();

            _stringOptionBuilder = new OptionBuilder<string>(_parser, _optionDefinition, _stringConverter, _optionContext);
        }

        [TestMethod]
        public void SetsIsBoolean()
        {
            new OptionBuilder<bool>(_parser, _optionDefinition, _stringConverter, _optionContext);
            _optionDefinition.IsBoolean.Should().BeTrue();
        }

        [TestMethod]
        public void IsBooleanIsFalseByDefault()
        {
            _optionDefinition.IsBoolean.Should().BeFalse();
        }

        [TestMethod]
        public void SetsDelegate()
        {
            bool wasCalled = false;

            _stringOptionBuilder.Do(pc => wasCalled = true);

            A.CallTo(() => _stringConverter.To(typeof (string), "1")).Returns("1");

            _optionDefinition.ValueSetter.Should().NotBeNull();

            _optionDefinition.ValueSetter.SetValue("1");

            wasCalled.Should().BeTrue();
        }
    }
}