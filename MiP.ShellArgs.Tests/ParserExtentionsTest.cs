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
        private Mock<IParser> _mock;
        private IParser _parser;

        [TestInitialize]
        public void Initialize()
        {
            _container = new TestContainer();

            _mock = new Mock<IParser>();
            _parser = _mock.Object;
        }

        [TestMethod]
        public void AutoWireInstance()
        {
            _parser.AutoWire(_container);

            _mock.Verify(x => x.AutoWire(_container, It.IsAny<Action<IAutoWireOptionBuilder<TestContainer>>>()));
        }

        [TestMethod]
        public void AutoWireDelegate()
        {
            _parser.AutoWire<TestContainer>(b => { });

            _mock.Verify(x => x.AutoWire(It.IsAny<TestContainer>(), It.IsAny<Action<IAutoWireOptionBuilder<TestContainer>>>()));
        }

        [TestMethod]
        public void AutoWireInstanceAndDelegate()
        {
            _parser.AutoWire(_container, b => { });

            _mock.Verify(x => x.AutoWire(_container, It.IsAny<Action<IAutoWireOptionBuilder<TestContainer>>>()));
        }

        public class TestContainer
        {
        }
    }
}