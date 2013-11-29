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
        private OptionContext _optionContext;

        [TestInitialize]
        public void Initialize()
        {
            _optionContext = new OptionContext();

            _tokenizer = new TokenConverter(new ArgumentFactory(new ParserSettings()));
        }

        [TestMethod]
        public void FirstPositional()
        {
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "x");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       Position = 2
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "x", "y");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       Position = 2,
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "x", "y", "z");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "-a", "x");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b"
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "-a", "x", "-b", "y");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "-a", "x", "y", "z");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsBoolean = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "-a");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionContext, "-a", "x", "y").ToArray(),
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                ex => Assert.IsTrue(ex.Message == "Expected an option instead of value 'y'."));
        }

        [TestMethod]
        public void ValueWithoutOptionAtEnd()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionContext, "x").ToArray(),
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                ex => Assert.IsTrue(ex.Message == "Expected an option instead of value 'x'."));
        }

        [TestMethod]
        public void CollectionOptionWithoutValueIsRemoved()
        {
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsCollection = true
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "-a", "-b", "x");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       IsCollection = true
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       IsCollection = true
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, new List<string>
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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b"
                                   });

            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionContext, "-a", "-b").ToArray(),
                ex => Assert.IsTrue(ex.Message == "Option 'a' has no value assigned."));
        }

        [TestMethod]
        public void OptionWithoutValueAtEnd()
        {
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a"
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b"
                                   });

            ExceptionAssert.Throws<ParsingException>(() => _tokenizer.ConvertToTokens(_optionContext, "-a", "x", "-b").ToArray(),
                ex => Assert.IsTrue(ex.Message.StartsWith("Option 'b' has no value assigned.")));
        }

        [TestMethod]
        public void AliasesAreTranslatedToOriginalName()
        {
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "Fullname",
                                       Aliases = new[] { "F", "Fn" }
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "-F", "x", "-FN", "y", "-Fullname", "z");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "a",
                                       Position = 1
                                   });
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "b",
                                       Position = 2
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, "-b", "x", "-a", "y");

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
            _optionContext.Add(new OptionDefinition
                                   {
                                       Name = "HelloWorld",
                                       Position = 1
                                   });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, new[] { "-helloworLD", "x" });

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
            _optionContext.Add(new OptionDefinition
            {
                Name = "HelloWorld",
                Position = 1
            });

            IEnumerable<Token> tokens = _tokenizer.ConvertToTokens(_optionContext, new List<string> { "-helloworLD", "x" });

            var expected = new List<Token>
                           {
                               Token.CreateOption("HelloWorld"),
                               Token.CreateValue("x"),
                           };

            CollectionAssert.AreEqual(expected, tokens.ToArray());
        }
    }
}