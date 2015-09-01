using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class ArgumentFactoryTest
    {
        private ArgumentFactory _factory;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new ArgumentFactory(new ParserSettings());
        }

        [TestMethod]
        public void NoArgumentGiven()
        {
            const string arg = "thisIs,Just: a value|=withNoArgument";
            Argument argument = _factory.Parse(arg);

            argument.ShouldBeEquivalentTo(new Argument
                                          {
                                              Name = string.Empty,
                                              Value = arg
                                          });
        }

        [TestMethod]
        public void ArgumentWithoutValue()
        {
            const string arg = "-ThisIsMyArgument";
            Argument argument = _factory.Parse(arg);

            argument.ShouldBeEquivalentTo(new Argument
                                          {
                                              Name = arg.Substring(1),
                                              Value = string.Empty
                                          });
        }

        [TestMethod]
        public void MultiplePrefixesAreNotBeRemoved()
        {
            const string arg = "-/-ThisIsMyArgumentWithMultiPrefixes";
            Argument argument = _factory.Parse(arg);

            argument.ShouldBeEquivalentTo(new Argument
                                          {
                                              Name = arg.Substring(1),
                                              Value = string.Empty
                                          });
        }

        [TestMethod]
        public void WhenAssignmentGivenNameIsParsed()
        {
            const string arg = "-name:value";
            Argument argument = _factory.Parse(arg);

            argument.ShouldBeEquivalentTo(new Argument
                                          {
                                              Name = "name"
                                          },
                o => o.Including(x => x.Name));
        }

        [TestMethod]
        public void AssignmentWithEmptyNameIsTreatedAsValue()
        {
            const string arg = "-:value";
            Argument argument = _factory.Parse(arg);

            argument.HasName.Should().BeFalse();
            argument.HasValue.Should().BeTrue();
            argument.Value.Should().Be(arg);
        }

        [TestMethod]
        public void AcceptStandAloneOptionPrefixAsValue()
        {
            const string arg = "/";
            Argument argument = _factory.Parse(arg);

            argument.HasName.Should().BeFalse();
            argument.HasValue.Should().BeTrue();
            argument.Value.Should().Be("/");
        }

        [TestMethod]
        public void WhenAssignmentGivenValueIsParsed()
        {
            const string arg = "-name:value";
            Argument argument = _factory.Parse(arg);

            argument.Value.Should().Be("value");
        }

        [TestMethod]
        public void EmptyAssignmentAreParsedToStringEmpty()
        {
            const string arg = "-name=";
            Argument argument = _factory.Parse(arg);

            argument.Value.Should().Be(string.Empty);
        }

        [TestMethod]
        public void SimpleBooleanAreParsed()
        {
            const string arg = "-name+";
            Argument argument = _factory.Parse(arg);

            argument.Name.Should().Be("name");
            argument.Value.Should().Be("+");
        }

        [TestMethod]
        public void ValueWithSimpleBooleanStayIntact()
        {
            const string arg = "-name=value+";
            Argument argument = _factory.Parse(arg);

            argument.Value.Should().Be("value+");
        }
    }
}