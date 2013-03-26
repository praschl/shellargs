using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class DelegatingPropertySetterTest
    {
        private StringConverter _stringConverter;

        [TestInitialize]
        public void Initialize()
        {
            _stringConverter = new StringConverter(new StringParserProvider());
        }

        [TestMethod]
        public void CallsDelegateWithValue()
        {
            int value = 0;
            var setter = new DelegatingPropertySetter<int>(_stringConverter, x => value = x);

            setter.SetValue("2");

            Assert.AreEqual(2, value);
        }

        [TestMethod]
        public void CallsOnValueSet()
        {
            var setter = new DelegatingPropertySetter<int>(_stringConverter, x => { });

            ValueSetEventArgs eventArgs = null;

            setter.ValueSet += (o, e) => eventArgs = e;
            setter.SetValue("1");

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(1, eventArgs.Value);
        }
    }
}