using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class SimpleParserTest
    {
        [TestMethod]
        public void SetsStringProperties()
        {
            var result = Parser.Parse<OptionalStringProperties>("a", "b");

            result.ShouldBeEquivalentTo(new OptionalStringProperties {S1 = "a", S2 = "b"});
        }

        [TestMethod]
        public void OmittingOptionalDoesNotFail()
        {
            var result = Parser.Parse<OptionalStringProperties>("a");

            result.ShouldBeEquivalentTo(new OptionalStringProperties {S1 = "a"});
        }

        [TestMethod]
        public void AllRequiredOptionsAreSet()
        {
            Action parse = () => Parser.Parse<RequiredStringProperties>("a");

            parse.ShouldThrow<ParsingException>()
                .WithMessage("The following option(s) are required, but were not given: [S2].");
        }

        [TestMethod]
        public void FailsWhenRequiredIsNotSet()
        {
            Action parse = () => Parser.Parse<RequiredStringProperties>("a", "-failplease");

            parse.ShouldThrow<ParsingException>()
                .WithMessage("'failplease' is not a valid option.");
        }

        [TestMethod]
        public void SimpleTypesAreWorking()
        {
            var result = Parser.Parse<SimpleTypes>("1", "2", "c", "eins", "eins,vier", "12/24/2010", "4.13:24:56.789", "Hi", "3", "4", "5", "6", "7.1", "8.2", "9.3", "10", "11", "12");

            var expected = new SimpleTypes
                           {
                               Int = 1,
                               Long = 2,
                               Char = 'c',
                               NormalEnumValue = NormalEnum.Eins,
                               FlagsEnumValue = FlagsEnum.Eins | FlagsEnum.Vier,
                               DateTime = new DateTime(2010, 12, 24),
                               TimeSpan = new TimeSpan(4, 13, 24, 56, 789),
                               String = "Hi",
                               Short = 3,
                               UInt = 4,
                               ULong = 5,
                               UShort = 6,
                               Decimal = new decimal(7.1),
                               Double = 8.2,
                               Float = 9.3f,
                               Integers = new List<int> {10, 11, 12}
                           };

            result.ShouldBeEquivalentTo(expected, o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void RequiredWithName()
        {
            var result = Parser.Parse<RequiredStringProperties>("v1", "-s3=v3", "-s2", "v2");

            result.ShouldBeEquivalentTo(new RequiredStringProperties {S1 = "v1", S2 = "v2", S3 = "v3"});
        }

        [TestMethod]
        public void RequiredArgumentWithoutValue()
        {
            Action parse = () => Parser.Parse<RequiredStringProperties>("v1", "-s2", "-s3=v3");

            parse.ShouldThrow<ParsingException>().WithMessage("Option 'S2' has no value assigned.");
        }

        [TestMethod]
        public void RequiredWithCollection()
        {
            var result = Parser.Parse<RequiredStringPropertiesWithCollection>("v1", "v2", "v3a", "v3b", "v3c");

            result.ShouldBeEquivalentTo(new RequiredStringPropertiesWithCollection
                                        {
                                            S1 = "v1",
                                            S2 = "v2",
                                            S3Collection = new List<string> {"v3a", "v3b", "v3c"}
                                        },
                o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void RequiredWithNameAndCollection()
        {
            var result = Parser.Parse<RequiredStringPropertiesWithCollection>("v1", "-S3Collection", "v3a", "v3b", "v3c", "-S2:v2");

            result.ShouldBeEquivalentTo(new RequiredStringPropertiesWithCollection
                                        {
                                            S1 = "v1",
                                            S2 = "v2",
                                            S3Collection = new List<string> {"v3a", "v3b", "v3c"}
                                        },
                o => o.WithStrictOrdering());
        }

        [TestMethod]
        public void UsesRegisteredParsers()
        {
            ParserSettings settings = new ParserSettings();
            settings.ParseTo<MyString>().With<StringToMyStringParser>();

            var result = Parser
                .Parse<MyStringOptions>(settings, "IExpectThis");

            result.ShouldBeEquivalentTo(new MyStringOptions
                                        {
                                            ThisIsMyString = new MyString
                                                             {
                                                                 TheString = "IExpectThis"
                                                             }
                                        });
        }

        [TestMethod]
        public void UsesRegisteredParserInstance()
        {
            ParserSettings settings = new ParserSettings();
            settings.ParseTo<MyString>().With(new StringToMyStringParser());

            var result = Parser
                .Parse<MyStringOptions>(settings, "IExpectThis");

            result.ShouldBeEquivalentTo(new MyStringOptions
            {
                ThisIsMyString = new MyString
                {
                    TheString = "IExpectThis"
                }
            });
        }

        //[TestMethod]
        public void DatesWithOneFormat()
        {
            // TODO: after implement format/pattern reactivate this test
            var result = Parser.Parse<DateTimeListOptions>("-dates", "{d.M.yyyy}", "1.2.2000", "3.4.2005", "25.11.2010");

            Assert.AreEqual(3, result.Dates.Count);
            Assert.AreEqual(new DateTime(2000, 2, 1), result.Dates[0]);
            Assert.AreEqual(new DateTime(2005, 4, 3), result.Dates[1]);
            Assert.AreEqual(new DateTime(2010, 11, 25), result.Dates[2]);
        }

        //[TestMethod]
        public void DatesWithDifferentFormat()
        {
            // TODO: after implement format/pattern reactivate this test

            var result = Parser.Parse<DateTimeListOptions>("-dates", "{d.M.yyyy}", "1.2.2000", "{M.d.yyyy}", "4.3.2005", "{yyyy-d.M}", "2010-25.11");

            Assert.AreEqual(3, result.Dates.Count);
            Assert.AreEqual(new DateTime(2000, 2, 1), result.Dates[0]);
            Assert.AreEqual(new DateTime(2005, 4, 3), result.Dates[1]);
            Assert.AreEqual(new DateTime(2010, 11, 25), result.Dates[2]);
        }

        [TestMethod]
        public void DefaultBooleanValue()
        {
            var result = Parser.Parse<BooleanOptions>("-true", "TRUE", "-false", "false");

            result.ShouldBeEquivalentTo(new BooleanOptions
                                        {
                                            True = true, False = false, Null = null
                                        });
        }

        [TestMethod]
        public void ShortBooleanValue()
        {
            var result = Parser.Parse<BooleanOptions>("-true+", "-false=-");

            result.ShouldBeEquivalentTo(new BooleanOptions
                                        {
                                            True = true, False = false, Null = null
                                        });
        }

        [TestMethod]
        public void MoreThanOneValueForNonCollectionFails()
        {
            Action parse = () => Parser.Parse<OptionalStringProperties>("-s1", "v1", "-s2", "v2", "v3");

            parse.ShouldThrow<ParsingException>().WithMessage("Expected an option instead of value 'v3'.");
        }

        [TestMethod]
        public void AliasesAreAccepted()
        {
            var result = Parser.Parse<AliasesOptions>("-v1", "aBc");
            result.ShouldBeEquivalentTo(new AliasesOptions {Value1 = "aBc"});

            result = Parser.Parse<AliasesOptions>("-z", "xYz");
            result.ShouldBeEquivalentTo(new AliasesOptions { Value1 = "xYz" });
        }

        [TestMethod]
        public void PassingValueToIgnoreOptionFails()
        {
            Action parse = () => Parser.Parse<IgnoredOption>("-value1", "v1", "-value2", "v2");
    
            parse.ShouldThrow<ParsingException>().WithMessage("'value2' is not a valid option.");
        }

        [TestMethod]
        public void SupportNonPositionalRequiredOptions()
        {
            Action parse = () => Parser.Parse<RequiredAndNonRequiredOption>();

            parse.ShouldThrow<ParsingException>()
                .WithMessage("The following option(s) are required, but were not given: [Required].");
        }

        [TestMethod]
        public void RequiredOptionWithDefaultNotGivenAtAllFails()
        {
            Action parse = () => Parser.Parse<RequiredAndNonRequiredOption>();

            parse.ShouldThrow<ParsingException>()
                .WithMessage("The following option(s) are required, but were not given: [Required].");
        }

        [TestMethod]
        public void BooleanWithoutValueNegatesValue()
        {
            var result = Parser.Parse<Booleans>("-True", "-False");

            result.ShouldBeEquivalentTo(new Booleans {True = false, False = true});
        }

        [TestMethod]
        public void NullableBooleansWithoutValueNegatesAndSetNullToTrue()
        {
            var instance = new BooleanOptions
                           {
                               False = true, True = false, Null = null
                           };

            Parser.Parse(instance, "-True", "-False", "-null");

            var expected = new BooleanOptions
                           {
                               False = false, True = true, Null = true
                           };

            instance.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void CanFillDifferentInstances()
        {
            var instance1 = new OptionalStringProperties
                            {
                                S1 = "instance 1"
                            };

            var instance2 = new OptionalStringProperties
                            {
                                S1 = "instance 2"
                            };

            Parser.Parse(instance1, "-s1=inst1");
            Parser.Parse(instance2, "-s1=inst2");

            instance1.ShouldBeEquivalentTo(new OptionalStringProperties {S1 = "inst1"});
            instance2.ShouldBeEquivalentTo(new OptionalStringProperties {S1 = "inst2"});
        }

        [TestMethod]
        public void WhenValueIsNotParsable()
        {
            Action parse = () => Parser.Parse<NumberOption>("-Number", "abc");

            parse.ShouldThrow<ParsingException>()
                .WithMessage("Could not parse value 'abc' to type System.Int32*.");
        }

        [TestMethod]
        public void StaticParseWithContainerAndSettings()
        {
            var optionalStringProperties = new OptionalStringProperties();

            Parser.Parse(optionalStringProperties, new ParserSettings(), "-S1", "1");

            optionalStringProperties.ShouldBeEquivalentTo(new OptionalStringProperties { S1 = "1" });
        }

        [TestMethod]
        public void CanUsePrivateProperties()
        {
            var container = Parser.Parse<PrivatePropertyContainer>("-Value", "Hello");

            container.GetValue().Should().Be("Hello");
        }

        [TestMethod]
        public void CanParseToDictionary()
        {
            var container = Parser.Parse<DictionaryContainer>("-Names", "a=b", "c:d", "-Numbers:e=5");

            var expected = new DictionaryContainer
                           {
                               Names = new Dictionary<string, string> {["a"] = "b", ["c"] = "d"},
                               Numbers = new Dictionary<string, int> {["e"] = 5}
                           };

            container.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void CanNotAddSameOptionTwiceIsCaseInsensitive()
        {
            var parser = new Parser();
            parser.RegisterOption("add").As<int>().Do(delegate { });

            Action parse =()=> parser.RegisterOption("Add").As<int>().Do(delegate { });

            parse.ShouldThrow<ParserInitializationException>()
                .WithMessage("The following names or aliases are not unique: [add].");
        }

        [TestMethod]
        public void PositionalPropertiesOutOfOrderDoesNotFail()
        {
            var parser = new Parser();

            Action registerContainer = () => parser.RegisterContainer<PositionalPropertiesOutOfOrder>();

            registerContainer.ShouldNotThrow();
        }

        [TestMethod]
        public void ParseToDictionaryDoubleKey()
        {
            var container = Parser.Parse<DictionaryContainer>("-Names", "a=b", "a:c");

            container.ShouldBeEquivalentTo(
                new DictionaryContainer
                {
                    Names = new Dictionary<string, string> {["a"] = "c"},
                    Numbers=new Dictionary<string, int>()
                });
        }

        #region Classes used by Test

        public class OptionalStringProperties
        {
            [Position(1)]
            public string S1 { get; set; }

            [Position(2)]
            public string S2 { get; set; }

            [Position(3)]
            public string S3 { get; set; }
        }

        public class RequiredStringProperties
        {
            [Required]
            [Position(1)]
            public string S1 { get; set; }

            [Required]
            [Position(2)]
            public string S2 { get; set; }

            [Position(3)]
            public string S3 { get; set; }
        }

        public class SimpleTypes
        {
            [Required]
            [Position(1)]
            public int Int { get; set; }

            [Required]
            [Position(2)]
            public long Long { get; set; }

            [Required]
            [Position(3)]
            public char Char { get; set; }

            [Required]
            [Position(4)]
            public NormalEnum NormalEnumValue { get; set; }

            [Required]
            [Position(5)]
            public FlagsEnum FlagsEnumValue { get; set; }

            [Required]
            [Position(6)]
            public DateTime DateTime { get; set; }

            [Required]
            [Position(7)]
            public TimeSpan TimeSpan { get; set; }

            [Required]
            [Position(8)]
            public string String { get; set; }

            [Required]
            [Position(9)]
            public short Short { get; set; }

            [Required]
            [Position(10)]
            public uint UInt { get; set; }

            [Required]
            [Position(11)]
            public ulong ULong { get; set; }

            [Required]
            [Position(12)]
            public ushort UShort { get; set; }

            [Required]
            [Position(13)]
            public decimal Decimal { get; set; }

            [Required]
            [Position(14)]
            public double Double { get; set; }

            [Required]
            [Position(15)]
            public float Float { get; set; }

            [Required]
            [Position(16)]
            public List<int> Integers { get; set; }
        }

        public enum NormalEnum
        {
            Null,
            Eins,
            Zwei
        }

        [Flags]
        public enum FlagsEnum
        {
            Eins = 0x1,
            Zwei = 0x2,
            Vier = 0x4
        }

        public class RequiredStringPropertiesWithCollection
        {
            [Required]
            [Position(1)]
            public string S1 { get; set; }

            [Required]
            [Position(2)]
            public string S2 { get; set; }

            [Position(3)]
            public List<string> S3Collection { get; set; }
        }

        public class UninitializedCollectionProperty
        {
            public ICollection<int> Numbers { get; set; }
        }

        public class MyStringOptions
        {
            [Required]
            [Position(1)]
            public MyString ThisIsMyString { get; set; }
        }

        public class MyString
        {
            public string TheString { get; set; }
        }

        public class StringToMyStringParser : StringParser
        {
            public override bool CanParseTo(Type targetType)
            {
                return true;
            }

            public override bool IsValid(Type targetType, string value)
            {
                return true;
            }

            public override object Parse(Type targetType, string value)
            {
                return new MyString
                       {
                           TheString = value
                       };
            }
        }

        public class DateTimeListOptions
        {
            public IList<DateTime> Dates { get; set; }
        }

        public class BooleanOptions
        {
            public bool? True { get; set; }

            public bool? False { get; set; }

            public bool? Null { get; set; }
        }

        public class AliasesOptions
        {
            [Aliases("V1", "Z")]
            public string Value1 { get; set; }
        }

        public class IgnoredOption
        {
            public string Value1 { get; set; }

            [IgnoreOption]
            public string Value2 { get; set; }
        }

        public class RequiredAndNonRequiredOption
        {
            [Required]
            [Aliases("r")]
            public string Required { get; set; }

            [Aliases("n")]
            public string NonRequired { get; set; }
        }

        public class Booleans
        {
            public Booleans()
            {
                True = true;
            }

            public bool True { get; set; }

            public bool False { get; set; }
        }

        public class NumberOption
        {
            public int Number { get; set; }
        }

        internal class PrivatePropertyContainer
        {
            public PrivatePropertyContainer()
            {
            }

            public PrivatePropertyContainer(string value)
            {
                Value = value;
            }

            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            // setter is required for test.
            private string Value { get; set; }

            public string GetValue()
            {
                return Value;
            }
        }

        public class DictionaryContainer
        {
            public Dictionary<string, string> Names { get; set; }
            public Dictionary<string, int> Numbers { get; set; }
        }

        public class PositionalPropertiesOutOfOrder
        {
            // Important:
            // DO NOT change the order of the properties, this is important for the test.

            [Position(3)]
            public string S1 { get; set; }

            [Position(2)]
            public string S2 { get; set; }

            [Position(1)]
            public string S3 { get; set; }
        }

        #endregion
    }
}