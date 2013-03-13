using System;
using System.Collections.Generic;

using MiP.ShellArgs.AutoWireAttributes;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class HelpGeneratorTests
    {
        private HelpGenerator _helpGenerator;
        private static PropertyReflector _reflector;
        private static object _optionsInstance;

        [TestInitialize]
        public void Initialize()
        {
            _optionsInstance = new StringOnlyOptions();
            var stringConverter = new StringConverter(null);

            _reflector = new PropertyReflector(stringConverter);

            _helpGenerator = new HelpGenerator(stringConverter);
            _helpGenerator.OptionPrefix = '-';
        }

        [TestMethod]
        public void OptionalNonPositional()
        {
            OptionDefinition info = GetPropertyInfo("J");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("[-J string]", help);
        }

        [TestMethod]
        public void OptionalPositional()
        {
            OptionDefinition info = GetPropertyInfo("C");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("[[-C] string]", help);
        }

        [TestMethod]
        public void RequiredPositional()
        {
            OptionDefinition info = GetPropertyInfo("A");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("[-A] string", help);
        }

        [TestMethod]
        public void RequiredNonPositional()
        {
            OptionDefinition info = GetPropertyInfo("H");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("-H string", help);
        }

        [TestMethod]
        public void OrderingManyOptions()
        {
            string help = _helpGenerator.GetParameterHelp(
                GetPropertyInfo("C"),
                GetPropertyInfo("H"),
                GetPropertyInfo("J"),
                GetPropertyInfo("B"),
                GetPropertyInfo("D"),
                GetPropertyInfo("A"),
                GetPropertyInfo("K"),
                GetPropertyInfo("I")
                );

            var helps = new List<string>
                        {
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("A")),
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("B")),
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("C")),
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("D")),
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("H")),
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("I")),
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("J")),
                            _helpGenerator.GetParameterHelp(GetPropertyInfo("K"))
                        };

            string fullHelp = string.Join(" ", helps);

            Assert.AreEqual(fullHelp, help);
        }

        [TestMethod]
        public void EnumHelp()
        {
            OptionDefinition info = GetPropertyInfo("Enum");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("-Enum (Eins|Zwei|Drei)", help);
        }

        [TestMethod]
        public void FlagsEnumHelp()
        {
            OptionDefinition info = GetPropertyInfo("FlagsEnum");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("-FlagsEnum (Eins,Zwei,Vier,Acht)", help);
        }

        [TestMethod]
        public void StringParserHelp()
        {
            OptionDefinition info = GetPropertyInfo("Date");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("-Date date", help);
        }

        [TestMethod]
        public void OptionAttributeHelp()
        {
            OptionDefinition info = GetPropertyInfo("DateRenamed");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("-DateRenamed renamed", help);
        }

        [TestMethod]
        public void CollectionHelp()
        {
            OptionDefinition info = GetPropertyInfo("Numbers");

            string help = _helpGenerator.GetParameterHelp(info);

            Assert.AreEqual("-Numbers int [...]", help);
        }

        private static OptionDefinition GetPropertyInfo(string name)
        {
            return _reflector.CreateOptionDefinition(typeof (StringOnlyOptions).GetProperty(name), _optionsInstance);
        }

        public class StringOnlyOptions
        {
            [Required]
            [Position(1)]
            public string A { get; set; }

            [Required]
            [Position(2)]
            public string B { get; set; }

            [Position(3)]
            public string C { get; set; }

            [Position(4)]
            public string D { get; set; }

            [Required]
            public string H { get; set; }

            [Required]
            public string I { get; set; }

            public string J { get; set; }

            public string K { get; set; }

            [Required]
            public EnumValues Enum { get; set; }

            [Required]
            public FlagsEnumValues FlagsEnum { get; set; }

            [Required]
            public DateTime Date { get; set; }

            [Required]
            [ValueDescription("renamed")]
            public DateTime DateRenamed { get; set; }

            [Required]
            public List<int> Numbers { get; set; }
        }

        public enum EnumValues
        {
            Eins,
            Zwei,
            Drei
        }

        [Flags]
        public enum FlagsEnumValues
        {
            Eins = 0x1,
            Zwei = 0x2,
            Vier = 0x4,
            Acht = 0x8
        }
    }
}