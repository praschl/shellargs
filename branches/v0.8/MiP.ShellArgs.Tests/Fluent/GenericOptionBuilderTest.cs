using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MiP.ShellArgs.Tests.Fluent
{
    [TestClass]
    public class GenericOptionBuilderTest
    {
        private OptionDefinition _optionDefinition;
        private OptionBuilder<string> _stringOptionBuilder;
        private Mock<IParserBuilder> _parserMock;
        private Mock<IStringConverter> _stringConverterMock;

        [TestInitialize]
        public void Initialize()
        {
            _parserMock = new Mock<IParserBuilder>();
            _stringConverterMock = new Mock<IStringConverter>();

            _optionDefinition = new OptionDefinition();

            _stringOptionBuilder = new OptionBuilder<string>(_parserMock.Object, _optionDefinition, _stringConverterMock.Object);
        }

        [TestMethod]
        public void SetsIsBoolean()
        {
            new OptionBuilder<bool>(_parserMock.Object, _optionDefinition, _stringConverterMock.Object);
            Assert.IsTrue(_optionDefinition.IsBoolean);
        }

        [TestMethod]
        public void DoesNotSetIsBoolean()
        {
            Assert.IsFalse(_optionDefinition.IsBoolean);
        }

        [TestMethod]
        public void SetsDelegate()
        {
            bool wasCalled = false;

            _stringOptionBuilder.Do(pc => wasCalled = true);

            _stringConverterMock.Setup(x => x.To(typeof (string), "1")).Returns("1");

            Assert.IsNotNull(_optionDefinition.ValueSetter);
            _optionDefinition.ValueSetter.SetValue("1");

            Assert.IsTrue(wasCalled);
        }
    }
}