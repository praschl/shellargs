using System;
using System.Collections.Generic;

using FluentAssertions;

using MiP.ShellArgs.Fluent;

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

            result1.Should().BeSameAs(_container1);
            result2.Should().BeSameAs(_container2);
        }

        [TestMethod]
        public void ResultToOutReturnsCorrectInstance()
        {
            TestContainer1 outResult1;
            TestContainer2 outResult2;
            
            _result.ResultTo(out outResult1);
            _result.ResultTo(out outResult2);

            outResult1.Should().BeSameAs(_container1);
            outResult2.Should().BeSameAs(_container2);
        }

        [TestMethod]
        public void FailsWhenTypeNotFound()
        {
            Action getResult = () => _result.Result<string>();

            getResult.ShouldThrow<KeyNotFoundException>()
                .WithMessage("Type System.String is not a known argument container type, add it with RegisterContainer<T>(), RegisterContainer<T>(T instance) or Parse<T>().");
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