using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.Implementation;

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

            Action validate = () => _validator.Validate(options);

            validate.ShouldThrow<ParserInitializationException>()
                .WithMessage("The following names or aliases are not unique: [A, B].");
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

            Action validate = () => _validator.Validate(options);

            validate.ShouldThrow<ParserInitializationException>()
                .WithMessage("The following options have no unique position: [A, B, C, D].");
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

            Action validate = () => _validator.Validate(options);

            validate.ShouldThrow<ParserInitializationException>()
                .WithMessage("Option with position 3 was found but position 2 is missing.");
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

            Action validate = () => _validator.Validate(options);

            validate.ShouldThrow<ParserInitializationException>()
                .WithMessage("Positional options must specify all required before optional options, but optional 'B' was followed by required 'C'.");
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

            Action validate = () => _validator.Validate(options);

            validate.ShouldThrow<ParserInitializationException>()
                .WithMessage("Only the last positional option may be a collection, but 'B' and 'C' are positional collection options.");
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

            Action validate = () => _validator.Validate(options);

            validate.ShouldThrow<ParserInitializationException>()
                .WithMessage("There are options which have no name. Use the .Named() method to give a name to an option");
        }
    }
}