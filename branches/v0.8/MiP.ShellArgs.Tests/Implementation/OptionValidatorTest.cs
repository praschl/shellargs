using System.Collections.Generic;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests.Implementation
{
    [TestClass]
    public class OptionValidatorTest
    {
        private OptionValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new OptionValidator();
        }

        [TestMethod]
        public void NamesAndAliasesAreUnique()
        {
            var options = new List<OptionDefinition>
                          {
                              new OptionDefinition
                              {
                                  Name = "A",
                                  Aliases = "B|C".Split('|')
                              },
                              new OptionDefinition
                              {
                                  Name = "D",
                                  Aliases = "B|E".Split('|')
                              },
                              new OptionDefinition
                              {
                                  Name = "A",
                                  Aliases = "F|G".Split('|')
                              }
                          };

            const string expectedMessage = "The following names or aliases are not unique: [A, B].";

            CallValidate(options, expectedMessage);
        }

        [TestMethod]
        public void PositionsAreUnique()
        {
            var options = new List<OptionDefinition>
                          {
                              new OptionDefinition
                              {
                                  Name = "A",
                                  Position = 1,
                              },
                              new OptionDefinition
                              {
                                  Name = "B",
                                  Position = 2,
                              },
                              new OptionDefinition
                              {
                                  Name = "C",
                                  Position = 1,
                              },
                              new OptionDefinition
                              {
                                  Name = "D",
                                  Position = 2,
                              }
                          };

            const string expectedMessage = "The following options have no unique position: [A, B, C, D].";

            CallValidate(options, expectedMessage);
        }

        [TestMethod]
        public void NoPositionIsMissing()
        {
            var options = new List<OptionDefinition>
                          {
                              new OptionDefinition
                              {
                                  Name = "A",
                                  Position = 1,
                              },
                              new OptionDefinition
                              {
                                  Name = "B",
                                  Position = 3,
                              },
                          };

            const string expectedMessage = "Option with position 3 was found but position 2 is missing.";

            CallValidate(options, expectedMessage);
        }
            
        [TestMethod]
        public void NoRequiredFollowsOptional()
        {
            var options = new List<OptionDefinition>
                          {
                              new OptionDefinition
                              {
                                  Name = "A",
                                  Position = 1,
                                  IsRequired = true,
                              },
                              new OptionDefinition
                              {
                                  Name = "B",
                                  Position = 2,
                                  IsRequired = false,
                              },
                              new OptionDefinition
                              {
                                  Name = "C",
                                  Position = 3,
                                  IsRequired = true,
                              },
                          };

            const string expectedMessage = "Positional options must specify all required before optional options, but optional 'B' was followed by required 'C'.";

            CallValidate(options, expectedMessage);
        }

        [TestMethod]
        public void OnlyLastPositionalIsCollection()
        {
            var options = new List<OptionDefinition>
                          {
                              new OptionDefinition
                              {
                                  Name = "A",
                                  Position = 1,
                              },
                              new OptionDefinition
                              {
                                  Name = "B",
                                  Position = 2,
                                  IsCollection = true,
                              },
                              new OptionDefinition
                              {
                                  Name = "C",
                                  Position = 3,
                                  IsCollection = true,
                              },
                          };

            const string expectedMessage = "Only the last positional option may be a collection, but 'B' and 'C' are positional collection options.";

            CallValidate(options, expectedMessage);
        }

        [TestMethod]
        public void AllOptionsMustHaveAName()
        {
            var options = new List<OptionDefinition>
                          {
                              new OptionDefinition
                              {
                                  Name = "",
                                  Aliases = "B|C".Split('|')
                              }
                          };

            const string expectedMessage = "There are options which have no name. Use the .Named() method to give a name to an option";

            CallValidate(options, expectedMessage);
        }

        private void CallValidate(ICollection<OptionDefinition> options, string expectedMessage)
        {
            ExceptionAssert.Throws<ParserInitializationException>(
                () => _validator.Validate(options),
                ex => Assert.AreEqual(expectedMessage, ex.Message));
        }
    }
}