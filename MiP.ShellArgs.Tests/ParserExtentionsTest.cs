using System;

using MiP.ShellArgs.Tests.TestHelpers;

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

            _mock.Verify(x => x.RegisterContainer(_container, It.IsAny<Action<IContainerBuilder<TestContainer>>>()));
        }

        [TestMethod]
        public void RegisterContainerDelegate()
        {
            _parser.RegisterContainer<TestContainer>(b => { });

            _mock.Verify(x => x.RegisterContainer(It.IsAny<TestContainer>(), It.IsAny<Action<IContainerBuilder<TestContainer>>>()));
        }

        [TestMethod]
        public void RegisterContainerInstanceAndDelegate()
        {
            _parser.RegisterContainer(_container, b => { });

            _mock.Verify(x => x.RegisterContainer(_container, It.IsAny<Action<IContainerBuilder<TestContainer>>>()));
        }

        public class TestContainer
        {
        }
    }
}