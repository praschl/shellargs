using System;
using System.Collections.Generic;

using MiP.ShellArgs.AutoWireAttributes;
using MiP.ShellArgs.StringConversion;
using MiP.ShellArgs.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class SimpleParserTest
    {
        [TestMethod]
        public void SetsStringProperties()
        {
            var result = Parser.Parse<OptionalStringProperties>("a", "b");

            Assert.AreEqual("a", result.S1);
            Assert.AreEqual("b", result.S2);
        }

        [TestMethod]
        public void OmittingOptionalDoesNotFail()
        {
            var result = Parser.Parse<OptionalStringProperties>("a");

            Assert.AreEqual("a", result.S1);
        }

        [TestMethod]
        public void AllRequiredOptionsAreSet()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<RequiredStringProperties>("a"),
                ex => Assert.AreEqual("The following option(s) are required, but were not given: [S2].", ex.Message));
        }

        [TestMethod]
        public void FailsWhenRequiredIsNotSet()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<RequiredStringProperties>("a", "-failplease"),
                ex => Assert.AreEqual("'failplease' is not a valid option.", ex.Message));
        }

        [TestMethod]
        public void SimpleTypesAreWorking()
        {
            var result = Parser.Parse<SimpleTypes>("1", "2", "c", "eins", "eins,vier", "12/24/2010", "4.13:24:56.789", "Hi", "3", "4", "5", "6", "7.1", "8.2", "9.3", "10", "11", "12");

            Assert.AreEqual(1, result.Int);
            Assert.AreEqual(2, result.Long);
            Assert.AreEqual('c', result.Char);
            Assert.AreEqual(NormalEnum.Eins, result.NormalEnumValue);
            Assert.AreEqual(FlagsEnum.Eins | FlagsEnum.Vier, result.FlagsEnumValue);
            Assert.AreEqual(new DateTime(2010, 12, 24), result.DateTime);
            Assert.AreEqual(new TimeSpan(4, 13, 24, 56, 789), result.TimeSpan);
            Assert.AreEqual("Hi", result.String);
            Assert.AreEqual(3, result.Short);
            Assert.AreEqual((uint)4, result.UInt);
            Assert.AreEqual((ulong)5, result.ULong);
            Assert.AreEqual((ushort)6, result.UShort);
            Assert.AreEqual(71, (int)(result.Decimal * 10));
            Assert.AreEqual(82, (int)(result.Double * 10));
            Assert.AreEqual(93, (int)(result.Float * 10));

            Assert.AreEqual(10, result.Integers[0]);
            Assert.AreEqual(11, result.Integers[1]);
            Assert.AreEqual(12, result.Integers[2]);
        }

        [TestMethod]
        public void RequiredWithName()
        {
            var result = Parser.Parse<RequiredStringProperties>("v1", "-s3=v3", "-s2", "v2");

            Assert.AreEqual("v1", result.S1);
            Assert.AreEqual("v2", result.S2);
            Assert.AreEqual("v3", result.S3);
        }

        [TestMethod]
        public void RequiredArgumentWithoutValue()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<RequiredStringProperties>("v1", "-s2", "-s3=v3"),
                ex => Assert.AreEqual("Option 'S2' has no value assigned.", ex.Message));
        }

        [TestMethod]
        public void RequiredWithCollection()
        {
            var result = Parser.Parse<RequiredStringPropertiesWithCollection>("v1", "v2", "v3a", "v3b", "v3c");

            Assert.AreEqual("v1", result.S1);
            Assert.AreEqual("v2", result.S2);

            Assert.AreEqual(3, result.S3Collection.Count);
            Assert.AreEqual("v3a", result.S3Collection[0]);
            Assert.AreEqual("v3b", result.S3Collection[1]);
            Assert.AreEqual("v3c", result.S3Collection[2]);
        }

        [TestMethod]
        public void RequiredWithNameAndCollection()
        {
            var result = Parser.Parse<RequiredStringPropertiesWithCollection>("v1", "-S3Collection", "v3a", "v3b", "v3c", "-S2:v2");

            Assert.AreEqual("v1", result.S1);
            Assert.AreEqual("v2", result.S2);

            Assert.AreEqual(3, result.S3Collection.Count);
            Assert.AreEqual("v3a", result.S3Collection[0]);
            Assert.AreEqual("v3b", result.S3Collection[1]);
            Assert.AreEqual("v3c", result.S3Collection[2]);
        }

        [TestMethod]
        public void UsesRegisteredParsers()
        {
            ParserSettings settings = new ParserSettings()
                .ParseTo<MyString>().With<StringToMyStringParser>();

            var result = Parser
                .Parse<MyStringOptions>(settings, "IExpectThis");

            Assert.IsNotNull(result.ThisIsMyString);
            Assert.AreEqual("IExpectThis", result.ThisIsMyString.TheString);
        }

        [TestMethod]
        public void UsesRegisteredParserInstance()
        {
            ParserSettings settings = new ParserSettings()
                .ParseTo<MyString>().With(new StringToMyStringParser());

            var result = Parser
                .Parse<MyStringOptions>(settings, "IExpectThis");

            Assert.IsNotNull(result.ThisIsMyString);
            Assert.AreEqual("IExpectThis", result.ThisIsMyString.TheString);
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

            Assert.AreEqual(true, result.True);
            Assert.AreEqual(false, result.False);
            Assert.IsNull(result.Null);
        }

        [TestMethod]
        public void ShortBooleanValue()
        {
            var result = Parser.Parse<BooleanOptions>("-true+", "-false=-");

            Assert.AreEqual(true, result.True);
            Assert.AreEqual(false, result.False);
            Assert.IsNull(result.Null);
        }

        [TestMethod]
        public void MoreThanOneValueForNonCollectionFails()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<OptionalStringProperties>("-s1", "v1", "-s2", "v2", "v3"),
                ex => Assert.AreEqual("Expected an option instead of value 'v3'.", ex.Message));
        }

        [TestMethod]
        public void AliasesAreAccepted()
        {
            var result = Parser.Parse<AliasesOptions>("-v1", "aBc");

            Assert.AreEqual("aBc", result.Value1);

            result = Parser.Parse<AliasesOptions>("-z", "xYz");
            Assert.AreEqual("xYz", result.Value1);
        }

        [TestMethod]
        public void PassingValueToIgnoreOptionFails()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<IgnoredOption>("-value1", "v1", "-value2", "v2"),
                ex => Assert.AreEqual("'value2' is not a valid option.", ex.Message));
        }

        [TestMethod]
        public void SupportNonPositionalRequiredOptions()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<RequiredAndNonRequiredOption>(),
                ex => Assert.AreEqual("The following option(s) are required, but were not given: [Required].", ex.Message));
        }

        [TestMethod]
        public void RequiredOptionWithDefaultNotGivenAtAllFails()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<RequiredAndNonRequiredOption>(),
                ex => Assert.AreEqual("The following option(s) are required, but were not given: [Required].", ex.Message));
        }

        [TestMethod]
        public void BooleanWithoutValueNegatesValue()
        {
            var result = Parser.Parse<Booleans>("-True", "-False");

            Assert.IsTrue(result.False);
            Assert.IsFalse(result.True);
        }

        [TestMethod]
        public void NullableBooleansWithoutValueNegatesAndSetNullToTrue()
        {
            var instance = new BooleanOptions
                           {
                               False = true,
                               True = false,
                               Null = null
                           };

            Parser.Parse(instance, "-True", "-False", "-null");

            Assert.AreEqual(true, instance.True);
            Assert.AreEqual(false, instance.False);
            Assert.AreEqual(true, instance.Null);
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

            Assert.AreEqual("inst1", instance1.S1);
            Assert.AreEqual("inst2", instance2.S1);
        }

        [TestMethod]
        public void WhenValueIsNotParsable()
        {
            ExceptionAssert.Throws<ParsingException>(
                () => Parser.Parse<NumberOption>("-Number", "abc"),
                ex =>
                {
                    Assert.IsTrue(ex.Message.StartsWith("Could not parse value 'abc' to type System.Int32"));
                    Assert.IsTrue(ex.Message.EndsWith("."));
                });
        }

        [TestMethod]
        public void StaticParseWithContainerAndSettings()
        {
            Parser.Parse(new OptionalStringProperties(), new ParserSettings(), new[] {"-S1", "1"});
        }

        [TestMethod]
        public void CanUsePrivateProperties()
        {
            var container = Parser.Parse<PrivatePropertyContainer>("-Value", "Hello");

            Assert.IsNotNull(container);
            Assert.AreEqual("Hello", container.GetValue());
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
            private string Value { get; set; }

            public string GetValue()
            {
                return Value;
            }
        }

        #endregion
    }
}