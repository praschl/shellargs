using MiP.ShellArgs.Implementation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class TokenTest
    {
        [TestMethod]
        public void CreateOption()
        {
            Token result = Token.CreateOption("hello");

            Assert.IsNotNull(result);
            Assert.AreEqual("hello", result.Name);
            Assert.IsTrue(result.IsOption);

            Assert.IsNull(result.Value);
        }

        [TestMethod]
        public void CreateValue()
        {
            Token result = Token.CreateValue("hello");

            Assert.IsNotNull(result);
            Assert.AreEqual("hello", result.Value);
            Assert.IsFalse(result.IsOption);

            Assert.IsNull(result.Name);
        }
    }
}