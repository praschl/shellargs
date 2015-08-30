using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class TokenTest
    {
        [TestMethod]
        public void CreateOption()
        {
            Token result = Token.CreateOption("hello");

            result.Should().NotBeNull();
            result.Name.Should().Be("hello");
            result.IsOption.Should().BeTrue();

            result.Value.Should().BeNull();
        }

        [TestMethod]
        public void CreateValue()
        {
            Token result = Token.CreateValue("hello");

            result.Should().NotBeNull();
            result.Value.Should().Be("hello");
            result.IsOption.Should().BeFalse();

            result.Name.Should().BeNull();
        }
    }
}