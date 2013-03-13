using System;
using System.Collections.Generic;

using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Fluent
{
    [TestClass]
    public class ParserResultTest
    {
        private IParserResult _result;
        private TestContainer1 _container1;
        private TestContainer2 _container2;

        [TestInitialize]
        public void Initialize()
        {
            _container1 = new TestContainer1();
            _container2 = new TestContainer2();

            _result = new ParserResult(new Dictionary<Type, object>
                                       {
                                           {typeof (TestContainer1), _container1},
                                           {typeof (TestContainer2), _container2}
                                       });
        }

        [TestMethod]
        public void ResultsGetsCorrectResults()
        {
            var result1 = _result.Result<TestContainer1>();
            var result2 = _result.Result<TestContainer2>();

            Assert.AreSame(_container1, result1);
            Assert.AreSame(_container2, result2);
        }

        [TestMethod]
        public void ResultToOutReturnsCorrectInstance()
        {
            TestContainer1 outResult1;
            TestContainer2 outResult2;
            
            _result.ResultTo(out outResult1);
            _result.ResultTo(out outResult2);

            Assert.AreSame(_container1, outResult1);
            Assert.AreSame(_container2, outResult2);
        }

        [TestMethod]
        public void FailsWhenTypeNotFound()
        {
            ExceptionAssert.Throws<KeyNotFoundException>(() => _result.Result<string>(),
                ex => Assert.AreEqual("Type System.String is not a known argument container type, add it with AutoWire<T>(), AutoWire<T>(T instance) or Parse<T>().", ex.Message));
        }

        #region Classes used by Test

        public class TestContainer1
        {
            public string AString1 { get; set; }
            public int ANumber1 { get; set; }
        }

        public class TestContainer2
        {
            public string AString2 { get; set; }
            public int ANumber2 { get; set; }
        }

        #endregion
    }
}