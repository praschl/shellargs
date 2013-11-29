using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class HelpGeneratorTests
    {
        private HelpGenerator _helpGenerator;
        private static PropertyReflector _reflector;
        private static object _optionsInstance;
        private IStringParserProvider _stringParserProvider;
        private OptionContext _context;

        [TestInitialize]
        public void Initialize()
        {
            _optionsInstance = new StringOnlyOptions();

            _stringParserProvider = new ParserSettings().ParserProvider;
            var stringConverter = new StringConverter(_stringParserProvider);

            _reflector = new PropertyReflector(stringConverter);

            _helpGenerator = new HelpGenerator(_stringParserProvider);
            _helpGenerator.OptionPrefix = '-';

            _context = new OptionContext();
        }

        [TestMethod]
        public void OptionalNonPositional()
        {
            _context.Add(GetPropertyInfo("J"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("[-J string]", help);
        }

        [TestMethod]
        public void OptionalPositional()
        {
            var info = GetPropertyInfo("C");
            info.Position = 1;
            _context.Add(info);

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("[[-C] string]", help);
        }

        [TestMethod]
        public void RequiredPositional()
        {
            _context.Add(GetPropertyInfo("A"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("[-A] string", help);
        }

        [TestMethod]
        public void RequiredNonPositional()
        {
            _context.Add(GetPropertyInfo("H"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("-H string", help);
        }

        [TestMethod]
        public void OrderingManyOptions()
        {
            _context.Add(GetPropertyInfo("J"));
            _context.Add(GetPropertyInfo("A"));
            _context.Add(GetPropertyInfo("H"));
            _context.Add(GetPropertyInfo("B"));
            _context.Add(GetPropertyInfo("C"));
            _context.Add(GetPropertyInfo("D"));
            _context.Add(GetPropertyInfo("K"));
            _context.Add(GetPropertyInfo("I"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            var orderedContext = new OptionContext();

            orderedContext.Add(GetPropertyInfo("A"));
            orderedContext.Add(GetPropertyInfo("B"));
            orderedContext.Add(GetPropertyInfo("C"));
            orderedContext.Add(GetPropertyInfo("D"));
            orderedContext.Add(GetPropertyInfo("H"));
            orderedContext.Add(GetPropertyInfo("I"));
            orderedContext.Add(GetPropertyInfo("J"));
            orderedContext.Add(GetPropertyInfo("K"));

            string fullHelp = _helpGenerator.GetParameterHelp(orderedContext.Definitions);

            Assert.AreEqual(fullHelp, help);
        }

        [TestMethod]
        public void EnumHelp()
        {
            _context.Add(GetPropertyInfo("Enum"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("-Enum (Eins|Zwei|Drei)", help);
        }

        [TestMethod]
        public void FlagsEnumHelp()
        {
            _context.Add(GetPropertyInfo("FlagsEnum"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("-FlagsEnum (Eins,Zwei,Vier,Acht)", help);
        }

        [TestMethod]
        public void StringParserHelp()
        {
            _context.Add(GetPropertyInfo("Date"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("-Date date", help);
        }

        [TestMethod]
        public void OptionAttributeHelp()
        {
            _context.Add(GetPropertyInfo("DateRenamed"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

            Assert.AreEqual("-DateRenamed renamed", help);
        }

        [TestMethod]
        public void CollectionHelp()
        {
            _context.Add(GetPropertyInfo("Numbers"));

            string help = _helpGenerator.GetParameterHelp(_context.Definitions);

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