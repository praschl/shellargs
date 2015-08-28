using System;
using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class FluentParserTests
    {
        [TestMethod]
        public void EventIsRaised()
        {
            var eventsRaised = new List<OptionValueParsedEventArgs>();

            var parser = new Parser();
            parser.OptionValueParsed += (o, e) => eventsRaised.Add(e);

            parser.RegisterContainer<RequiredAndNonRequiredOption>();

            parser.Parse("-r:V1", "-n:V2");

            OptionValueParsedEventArgs[] expectedEvents =
            {
                new OptionValueParsedEventArgs(null, "Required", "V1"),
                new OptionValueParsedEventArgs(null, "NonRequired", "V2")
            };

            eventsRaised.ShouldAllBeEquivalentTo(expectedEvents, o => o.Excluding(x => x.ParserBuilder));
        }

        [TestMethod]
        public void EventIsRaisedForOption()
        {
            var eventsRaised = new List<ParsingContext<RequiredAndNonRequiredOption, string>>();

            var parser = new Parser();
            parser.RegisterContainer<RequiredAndNonRequiredOption>()
                .With(x => x.NonRequired)
                .Do(eventsRaised.Add);
            parser.Parse("-r:V1", "-n:V2");

            OptionValueParsedEventArgs[] expectedEvents =
            {
                new OptionValueParsedEventArgs(null, "NonRequired", "V2")
            };

            eventsRaised.ShouldAllBeEquivalentTo(expectedEvents,
                o => o.Excluding(x => x.Parser).Excluding(x => x.Container));
        }

        [TestMethod]
        public void EventIsRaisedForRenamedOption()
        {
            var eventsRaised = new List<ParsingContext<RenamedOption, string>>();

            var parser = new Parser();
            parser.RegisterContainer<RenamedOption>()
                .With(x => x.OriginalName)
                .Do(eventsRaised.Add);
            parser.Parse("-newName:Value");

            OptionValueParsedEventArgs[] expectedEvents =
            {
                new OptionValueParsedEventArgs(null, "newName", "Value")
            };

            eventsRaised.ShouldAllBeEquivalentTo(expectedEvents,
                o => o.Excluding(x => x.Parser).Excluding(x => x.Container));
        }

        [TestMethod]
        public void OverriddenPrefixesAreUsed()
        {
            var eventsRaised = new List<OptionValueParsedEventArgs>();

            var settings = new ParserSettings();
            settings.PrefixWith('+');

            var parser = new Parser(settings);
            parser.OptionValueParsed += (o, e) => eventsRaised.Add(e);
            parser.RegisterOption("Hello").As<string>().Do(x => { });

            parser.Parse("+Hello", "World");

            OptionValueParsedEventArgs[] expectedEvents =
            {
                new OptionValueParsedEventArgs(null, "Hello", "World")
            };

            eventsRaised.ShouldAllBeEquivalentTo(expectedEvents,
                o => o.Excluding(x => x.ParserBuilder));
        }

        [TestMethod]
        public void OverriddenAssignmentsAreUsed()
        {
            var eventsRaised = new List<OptionValueParsedEventArgs>();

            var settings = new ParserSettings();
            settings.AssignWith('+');

            var parser = new Parser(settings);
            parser.OptionValueParsed += (o, e) => eventsRaised.Add(e);
            parser.RegisterOption("Hello").As<string>().Do(x => { });

            parser.Parse("-Hello+World");

            OptionValueParsedEventArgs[] expectedEvents =
            {
                new OptionValueParsedEventArgs(null, "Hello", "World")
            };

            eventsRaised.ShouldAllBeEquivalentTo(expectedEvents,
                o => o.Excluding(x => x.ParserBuilder));
        }

        [TestMethod]
        public void ShortHelpIsGenerated()
        {
            var settings = new ParserSettings();
            settings.PrefixWith('+');

            var parser = new Parser(settings);
            parser.RegisterOption("Hello").As<string>().Do(x => { });

            string help = parser.GetShortHelp();
            help.Should().Be("[+Hello string]");
        }

        [TestMethod]
        public void ShortHelpIsGeneratedWithRenamedOption()
        {
            var settings = new ParserSettings();
            settings.PrefixWith('+');

            var parser = new Parser(settings);
            parser.RegisterOption("Hello").ValueDescription("something").As<string>().Do(x => { });

            string help = parser.GetShortHelp();
            help.Should().Be("[+Hello something]");
        }

        [TestMethod]
        public void TwoInstancesWithDifferentEvents()
        {
            ParsingContext<TestContainer1, string> stringContext1 = null;
            ParsingContext<TestContainer1, int> intContext1 = null;
            ParsingContext<TestContainer2, string> stringContext2 = null;
            ParsingContext<TestContainer2, int> intContext2 = null;

            var parser = new Parser();

            parser.RegisterContainer<TestContainer1>()
                .With(c => c.AString1).Do(pc => stringContext1 = pc)
                .With(c => c.ANumber1).Do(pc => intContext1 = pc);

            parser.RegisterContainer<TestContainer2>()
                .With<string>("AString2").Do(pc => stringContext2 = pc)
                .With<int>("ANumber2").Do(pc => intContext2 = pc);

            parser.Parse("-AString1", "1", "-AString2", "2", "-ANumber1", "1", "-ANumber2", "2");

            var expectedStringContext1 = new ParsingContext<TestContainer1, string>(null, null, "AString1", "1");
            var expectedIntContext1 = new ParsingContext<TestContainer1, int>(null, null, "ANumber1", 1);
            var expectedStringContext2 = new ParsingContext<TestContainer2, string>(null, null, "AString2", "2");
            var expectedIntContext2 = new ParsingContext<TestContainer1, int>(null, null, "ANumber2", 2);

            stringContext1.ShouldBeEquivalentTo(expectedStringContext1, o => o.Excluding(x => x.Container).Excluding(x => x.Parser));
            intContext1.ShouldBeEquivalentTo(expectedIntContext1, o => o.Excluding(x => x.Container).Excluding(x => x.Parser));
            stringContext2.ShouldBeEquivalentTo(expectedStringContext2, o => o.Excluding(x => x.Container).Excluding(x => x.Parser));
            intContext2.ShouldBeEquivalentTo(expectedIntContext2, o => o.Excluding(x => x.Container).Excluding(x => x.Parser));

            stringContext1.Container.Should().BeSameAs(intContext1.Container);
            stringContext2.Container.Should().BeSameAs(intContext2.Container);
        }

        [TestMethod]
        public void TwoRegisterOptions()
        {
            int actual = 0;

            var parser = new Parser();

            parser.RegisterOption("add").As<int>()
                .Do(pc => actual += pc.Value);

            parser.RegisterOption("sub").As<int>()
                .Do(pc => actual -= pc.Value);

            parser.Parse("-add", "1", "-add", "2", "-sub", "10", "-add", "4");

            actual.Should().Be(-3);
        }

        [TestMethod]
        public void RegisterContainerWithCollection()
        {
            var values = new List<string>();

            var parser = new Parser();

            parser.RegisterContainer<CollectionContainer>()
                .With(c => c.Values.CurrentValue())
                .Do(pc => values.Add(pc.Value));

            var result = parser.Parse("-a", "1", "2", "3")
                .Result<CollectionContainer>();

            var expectation = new[] {"1", "2", "3"};

            values.ShouldAllBeEquivalentTo(expectation);
            result.Values.ShouldAllBeEquivalentTo(expectation);
        }

        [TestMethod]
        public void RegisterContainerSameTypeTwice()
        {
            Action doubleRegister = () =>
                                    {
                                        var parser = new Parser();
                                        parser.RegisterContainer<TestContainer1>();
                                        parser.RegisterContainer<TestContainer1>();
                                    };

            doubleRegister.ShouldThrow<ParserInitializationException>()
                .WithMessage($"Parser already knows a container of type {typeof (TestContainer1)}.");
        }

        [TestMethod]
        public void RegisterContainerSameInstanceTwice()
        {
            Action sameInstanceRegistration = () =>
                                              {
                                                  var parser = new Parser();
                                                  parser.RegisterContainer<TestContainer1>();
                                                  parser.RegisterContainer(new TestContainer1());
                                              };

            sameInstanceRegistration.ShouldThrow<ParserInitializationException>()
                .WithMessage($"Parser already knows a container of type {typeof (TestContainer1)}.");
        }

        [TestMethod]
        public void RegisterContainerWithoutInstance()
        {
            var parser = new Parser();
            parser.RegisterContainer<TestContainer1>(null);

            var result = parser.Parse().Result<TestContainer1>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void RegisterOptionUnNamed()
        {
            var list = new List<int>();

            var parser = new Parser();

            parser.RegisterOption().Named("add")
                .Collection
                .As<int>()
                .Do(context => list.Add(context.Value));

            parser.Parse("-add", "1", "2", "3");

            list.ShouldAllBeEquivalentTo(new[] {1, 2, 3});
        }

        [TestMethod]
        public void RegisterOptionCanParseKeyValuePairs()
        {
            var properties = new Dictionary<string, string>();

            var parser = new Parser();

            parser.RegisterOption().Named("p")
                .Collection
                .As<KeyValuePair<string, string>>()
                .Do(context => properties[context.Value.Key] = context.Value.Value);

            parser.Parse("/p:a=b", "/p:c=d", "/p:c=e");

            var expectation = new Dictionary<string, string>
                              {
                                  ["a"] = "b",
                                  ["c"] = "e"
                              };

            properties.ShouldAllBeEquivalentTo(expectation);
        }

        #region Classes used by Test

        public class RequiredAndNonRequiredOption
        {
            [Required]
            [Aliases("r")]
            public string Required { get; set; }

            [Aliases("n")]
            public string NonRequired { get; set; }
        }

        public class RenamedOption
        {
            [Option("newName")]
            public string OriginalName { get; set; }
        }

        public class TestContainer1
        {
            public string AString1 { get; set; }
            public int ANumber1 { get; set; }
        }

        public class TestContainer2
        {
            public string AString2 { get; set; }
            public int ANumber2 { get; set; }
        }

        public class CollectionContainer
        {
            [Aliases("a")]
            public List<string> Values { get; set; }
        }

        #endregion
    }
}