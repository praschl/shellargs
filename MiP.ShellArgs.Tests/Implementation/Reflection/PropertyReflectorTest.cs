using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

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

            options.ShouldAllBeEquivalentTo(expected);
        }

        [TestMethod]
        public void GeneratesCorrectSetters()
        {
            ICollection<OptionDefinition> options = _reflector.CreateOptionDefinitions(typeof (SetterProperties), new SetterProperties());

            options.First(o => o.Name == "A").ValueSetter.GetType().Should().Be(typeof (DefaultPropertySetter));
            options.First(o => o.Name == "B").ValueSetter.GetType().Should().Be(typeof (CollectionPropertySetter));
            options.First(o => o.Name == "C").ValueSetter.GetType().Should().Be(typeof (BooleanPropertySetter));
        }

        [TestMethod]
        public void ReadsOptionAttribute()
        {
            OptionDefinition option = _reflector.CreateOptionDefinitions(typeof (AttributedProperties), new AttributedProperties()).First();

            OptionDefinition expected = new OptionDefinition
                                        {
                                            Name = "NewName",
                                            Aliases =new List<string>
                                                     {
                                                "a",
                                                "b",
                                                "c"
                                            },
                                            Position = 1,
                                            IsRequired = true
                                        };

            option.ShouldBeEquivalentTo(expected, o => o.Excluding(x => x.ValueSetter));
        }

        [TestMethod]
        public void FlagsAreSetCorrectly()
        {
            ICollection<OptionDefinition> options = _reflector.CreateOptionDefinitions(typeof (SetterProperties), new SetterProperties());

            OptionDefinition[] expected =
            {
                new OptionDefinition {Name = "A",IsBoolean = false, IsCollection = false},
                new OptionDefinition {Name = "B",IsBoolean = false, IsCollection = true},
                new OptionDefinition {Name = "C",IsBoolean = true, IsCollection = false},
                new OptionDefinition {Name = "D",IsBoolean = true, IsCollection = true}
            };

            options.ShouldAllBeEquivalentTo(expected, o => o.Excluding(x => x.ValueSetter));
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