using System.Collections.Generic;
using System.Linq;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation.Reflection
{
    [TestClass]
    public class PropertyReflectorTest
    {
        private PropertyReflector _reflector;

        [TestInitialize]
        public void Initialize()
        {
            _reflector = new PropertyReflector(new StringConverter(new ParserSettings().ParserProvider));
        }

        [TestMethod]
        public void FindsAllRelevantProperties()
        {
            string[] options = _reflector.CreateOptionDefinitions(typeof (RelevantProperties), new RelevantProperties()).Select(o => o.Name).ToArray();

            var expected = new[] {"A", "B", "D", "E", "F", "G", "I"};

            string[] expectedButNotFound = expected.Except(options).ToArray();
            if (expectedButNotFound.Any())
                Assert.Fail("Expected properties were not found: " + string.Join(", ", expectedButNotFound));

            string[] foundButNotExpected = options.Except(expected).ToArray();
            if (foundButNotExpected.Any())
                Assert.Fail("Unexpected properties were found: " + string.Join(", ", foundButNotExpected));
        }

        [TestMethod]
        public void GeneratesCorrectSetters()
        {
            ICollection<OptionDefinition> options = _reflector.CreateOptionDefinitions(typeof (SetterProperties), new SetterProperties());

            Assert.AreEqual(typeof (DefaultPropertySetter), options.First(o => o.Name == "A").ValueSetter.GetType());
            Assert.AreEqual(typeof (CollectionPropertySetter), options.First(o => o.Name == "B").ValueSetter.GetType());
            Assert.AreEqual(typeof (BooleanPropertySetter), options.First(o => o.Name == "C").ValueSetter.GetType());
        }

        [TestMethod]
        public void ReadsOptionAttribute()
        {
            OptionDefinition option = _reflector.CreateOptionDefinitions(typeof (AttributedProperties), new AttributedProperties()).First();

            Assert.AreEqual("NewName", option.Name);
            CollectionAssert.AreEquivalent(new[] {"a", "b", "c"}, option.Aliases.ToArray());
            Assert.AreEqual(1, option.Position);
            Assert.AreEqual(true, option.IsRequired);
        }

        [TestMethod]
        public void FlagsAreSetCorrectly()
        {
            ICollection<OptionDefinition> options = _reflector.CreateOptionDefinitions(typeof (SetterProperties), new SetterProperties());

            OptionDefinition option = options.First(o => o.Name == "A");
            Assert.IsFalse(option.IsBoolean);
            Assert.IsFalse(option.IsCollection);

            option = options.First(o => o.Name == "B");
            Assert.IsFalse(option.IsBoolean);
            Assert.IsTrue(option.IsCollection);

            option = options.First(o => o.Name == "C");
            Assert.IsTrue(option.IsBoolean);
            Assert.IsFalse(option.IsCollection);

            option = options.First(o => o.Name == "D");
            Assert.IsTrue(option.IsBoolean);
            Assert.IsTrue(option.IsCollection);
        }

        private class RelevantProperties
        {
            public string A { get; set; }
            public string B { get; private set; }
            public string C { get { return null; } } // this must be ignored because of readonly
            internal int D { get; set; }
            internal int? E { get; set; }
            protected string F { get; set; }
            private string G { get; set; }
            public static string H { get; set; } // this must be ignored because of static
            public ICollection<string> I { get { return new List<string>(); } } // readonly, but should not be ignored because of ICollection
            [IgnoreOption]
            public string J { get; set; }
        }

        private class SetterProperties
        {
            public string A { get; set; }
            public ICollection<string> B { get; set; }
            public bool C { get; set; }
            public ICollection<bool> D { get; set; }
        }

        private class AttributedProperties
        {
            [Option("NewName")]
            [Aliases("a", "b", "c")]
            [Required]
            [Position(1)]
            public string A { get; set; }
        }
    }
}