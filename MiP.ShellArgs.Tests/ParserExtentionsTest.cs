using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class ParserExtentionsTest
    {
        private TestContainer _container;
        private Mock<IParserBuilder> _mock;
        private IParserBuilder _parser;

        [TestInitialize]
        public void Initialize()
        {
            _container = new TestContainer();

            _mock = new Mock<IParserBuilder>();
            _parser = _mock.Object;
        }

        [TestMethod]
        public void RegisterContainerInstance()
        {
            _parser.RegisterContainer(_container);

            _mock.Verify(x => x.RegisterContainer(_container));
        }

        public class TestContainer
        {
        }
    }
}