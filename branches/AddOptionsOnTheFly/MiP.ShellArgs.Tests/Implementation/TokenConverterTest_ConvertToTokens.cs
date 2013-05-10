using System.Collections.Generic;
using System.Linq;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class TokenConverterTest_ConvertToTokens
    {
        private TokenConverter _tokenizer;
        private List<OptionDefinition> _optionDefinitions;

        [TestInitialize]
        public void Initialize()
        {
            _optionDefinitions = new List<OptionDefinition>();

            _tokenizer = new TokenConverter(new ArgumentFactory(new ParserSettings()));
        }

        [TestMethod]
        public void FirstPositional()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "x");

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue("x")
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void SecondPositional()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       Position = 2
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "x", "y");

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue("x"),
                               Token.CreateOption("b"),
                               Token.CreateValue("y")
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void CollectionPositional()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       Position = 2,
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "x", "y", "z");

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue("x"),
                               Token.CreateOption("b"),
                               Token.CreateValue("y"),
                               Token.CreateValue("z")
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void FirstNamed()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "-a", "x");

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue("x")
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void SecondNamed()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b"
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "-a", "x", "-b", "y");

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue("x"),
                               Token.CreateOption("b"),
                               Token.CreateValue("y")
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void CollectionNamed()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "-a", "x", "y", "z");

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue("x"),
                               Token.CreateValue("y"),
                               Token.CreateValue("z")
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void NamedBooleanToggle()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsBoolean = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "-a");

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue(TokenConverter.ToggleBoolean),
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void ValueWithoutOptionInMiddle()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });

            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionDefinitions, "-a", "x", "y"),
                ex => Assert.IsTrue(ex.Message == "Expected an option instead of value 'y'."));
        }

        [TestMethod]
        public void ValueWithoutOptionAtEnd()
        {
            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionDefinitions, "x"),
                ex => Assert.IsTrue(ex.Message == "Expected an option instead of value 'x'."));
        }

        [TestMethod]
        public void CollectionOptionWithoutValueIsRemoved()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsCollection = true
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "-a", "-b", "x");

            var expected = new List<Token>
                           {
                               Token.CreateOption("b"),
                               Token.CreateValue("x"),
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void CollectionOptionWithoutValueAtEndIsRemoved()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsCollection = true
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, new List<string>
                                                                            {
                                                                                "-a",
                                                                                "x",
                                                                                "-b"
                                                                            });

            var expected = new List<Token>
                           {
                               Token.CreateOption("a"),
                               Token.CreateValue("x"),
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void OptionWithoutValue()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b"
                                   });

            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionDefinitions, "-a", "-b"),
                ex => Assert.IsTrue(ex.Message == "Option 'a' has no value assigned."));
        }

        [TestMethod]
        public void OptionWithoutValueAtEnd()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b"
                                   });

            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionDefinitions, "-a", "x", "-b"),
                ex => Assert.IsTrue(ex.Message.StartsWith("Option 'b' has no value assigned.")));
        }

        [TestMethod]
        public void AliasesAreTranslatedToOriginalName()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "Fullname",
                                       Aliases = new[] { "F", "Fn" }
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "-F", "x", "-FN", "y", "-Fullname", "z");

            var expected = new List<Token>
                           {
                               Token.CreateOption("Fullname"),
                               Token.CreateValue("x"),
                               Token.CreateOption("Fullname"),
                               Token.CreateValue("y"),
                               Token.CreateOption("Fullname"),
                               Token.CreateValue("z")
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void PositionalsMustBeUsableAsNamed()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       Position = 2
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, "-b", "x", "-a", "y");

            var expected = new List<Token>
                           {
                               Token.CreateOption("b"),
                               Token.CreateValue("x"),
                               Token.CreateOption("a"),
                               Token.CreateValue("y"),
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void NameIsCasedCorrectly()
        {
            _optionDefinitions.Add(new OptionDefinition
                                   {
                                       Name = "HelloWorld",
                                       Position = 1
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, new[] { "-helloworLD", "x" });

            var expected = new List<Token>
                           {
                               Token.CreateOption("HelloWorld"),
                               Token.CreateValue("x"),
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }

        [TestMethod]
        public void OverloadIEnumerable()
        {
            _optionDefinitions.Add(new OptionDefinition
            {
                Name = "HelloWorld",
                Position = 1
            });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionDefinitions, new List<string> { "-helloworLD", "x" });

            var expected = new List<Token>
                           {
                               Token.CreateOption("HelloWorld"),
                               Token.CreateValue("x"),
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }
    }
}