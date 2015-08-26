using FakeItEasy;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class ParserExtentionsTest
    {
        private TestContainer _container;
        private IParserBuilder _parser;

        [TestInitialize]
        public void Initialize()
        {
            _container = new TestContainer();

            _parser = A.Fake<IParserBuilder>();
        }

        [TestMethod]
        public void RegisterContainerInstance()
        {
            _parser.RegisterContainer(_container);

            A.CallTo(() => _parser.RegisterContainer(_container)).MustHaveHappened();
        }

        public class TestContainer
        {
        }
    }
}