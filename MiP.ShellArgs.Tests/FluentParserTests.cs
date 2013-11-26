using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.Tests.TestHelpers;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class FluentParserTests
    {
        [TestMethod]
        public void EventIsRaised()
        {
            var eventsByExtensionMethod = new List<ParsingContext<object>>();

            var parser = new Parser();
            parser.OnOptionParsed(eventsByExtensionMethod.Add);
            parser.RegisterContainer<RequiredAndNonRequiredOption>();

            parser.Parse("-r:V1", "-n:V2");

            AssertEventsRaised(eventsByExtensionMethod, "extension");
        }

        private static void AssertEventsRaised(IList<ParsingContext<object>> eventsRaised, string message)
        {
            Assert.AreEqual(2, eventsRaised.Count, message);
            Assert.AreEqual("Required", eventsRaised[0].Option, message);
            Assert.AreEqual("V1", eventsRaised[0].Value, message);

            Assert.AreEqual("NonRequired", eventsRaised[1].Option, message);
            Assert.AreEqual("V2", eventsRaised[1].Value, message);
        }

        [TestMethod]
        public void EventIsRaisedForOption()
        {
            var eventsRaised = new List<ParsingContext<RequiredAndNonRequiredOption, string>>();

            var parser = new Parser();
            parser.RegisterContainer<RequiredAndNonRequiredOption>(c => c.With(x => x.NonRequired).Do(eventsRaised.Add));
            parser.Parse("-r:V1", "-n:V2");

            Assert.AreEqual(1, eventsRaised.Count);
            Assert.AreEqual("V2", eventsRaised[0].Value);
        }

        [TestMethod]
        public void EventIsRaisedForRenamedOption()
        {
            var eventsRaised = new List<ParsingContext<RenamedOption, string>>();

            var parser = new Parser();
            parser.RegisterContainer<RenamedOption>(c => c.With(x => x.OriginalName).Do(eventsRaised.Add));
            parser.Parse("-newName:Value");

            Assert.AreEqual(1, eventsRaised.Count);
            Assert.AreEqual("Value", eventsRaised[0].Value);
        }

        [TestMethod]
        public void OverriddenPrefixesAreUsed()
        {
            var eventsRaised = new List<ParsingContext<object>>();

            var settings = new ParserSettings();
            settings.PrefixWith('+');

            var parser = new Parser(settings);
            parser.RegisterOption("Hello", b => b.As<string>().Do(x => { }));
            parser.OnOptionParsed(eventsRaised.Add);

            parser.Parse("+Hello", "World");

            Assert.AreEqual(1, eventsRaised.Count);
            Assert.AreEqual("Hello", eventsRaised[0].Option);
            Assert.AreEqual("World", eventsRaised[0].Value);
        }

        [TestMethod]
        public void OverriddenAssignmentsAreUsed()
        {
            var eventsRaised = new List<ParsingContext<object>>();

            var settings = new ParserSettings();
            settings.AssignWith('+');

            var parser = new Parser(settings);
            parser.RegisterOption("Hello", b => b.As<string>().Do(x => { }));
            parser.OnOptionParsed(eventsRaised.Add);

            parser.Parse("-Hello+World");

            Assert.AreEqual(1, eventsRaised.Count);
            Assert.AreEqual("Hello", eventsRaised[0].Option);
            Assert.AreEqual("World", eventsRaised[0].Value);
        }

        [TestMethod]
        public void ShortHelpIsGenerated()
        {
            var settings = new ParserSettings();
            settings.PrefixWith('+');

            var parser = new Parser(settings);
            parser.RegisterOption("Hello", b => b.As<string>().Do(x => { }));

            string help = parser.GetShortHelp();
            Assert.AreEqual("[+Hello string]", help);
        }

        [TestMethod]
        public void ShortHelpIsGeneratedWithRenamedOption()
        {
            var settings = new ParserSettings();
            settings.PrefixWith('+');

            var parser = new Parser(settings);
            parser.RegisterOption("Hello", b => b.ValueDescription("something").As<string>().Do(x => { }));

            string help = parser.GetShortHelp();
            Assert.AreEqual("[+Hello something]", help);
        }

        [TestMethod]
        public void TwoInstancesWithDifferentEvents()
        {
            ParsingContext<TestContainer1, string> stringContext1 = null;
            ParsingContext<TestContainer1, int> intContext1 = null;
            ParsingContext<TestContainer2, string> stringContext2 = null;
            ParsingContext<TestContainer2, int> intContext2 = null;

            var parser = new Parser();

            parser.RegisterContainer<TestContainer1>(
                aw => aw.With(c => c.AString1).Do(pc => stringContext1 = pc)
                    .With(c => c.ANumber1).Do(pc => intContext1 = pc));

            parser.RegisterContainer<TestContainer2>(
                aw => aw.With<string>("AString2").Do(pc => stringContext2 = pc)
                    .With<int>("ANumber2").Do(pc => intContext2 = pc));

            parser.Parse("-AString1", "1", "-AString2", "2", "-ANumber1", "1", "-ANumber2", "2");

            Assert.AreEqual("AString1", stringContext1.Option);
            Assert.AreEqual("1", stringContext1.Value);
            Assert.AreEqual("ANumber1", intContext1.Option);
            Assert.AreEqual(1, intContext1.Value);
            Assert.AreEqual("AString2", stringContext2.Option);
            Assert.AreEqual("2", stringContext2.Value);
            Assert.AreEqual("ANumber2", intContext2.Option);
            Assert.AreEqual(2, intContext2.Value);

            Assert.AreSame(stringContext1.Container, intContext1.Container);
            Assert.AreSame(stringContext2.Container, intContext2.Container);
        }

        [TestMethod]
        public void TwoRegisterOptions()
        {
            int actual = 0;

            var parser = new Parser();

            parser.RegisterOption("add",
                b => b.As<int>()
                    .Do(pc => actual += pc.Value));

            parser.RegisterOption("sub",
                b => b.As<int>()
                    .Do(pc => actual -= pc.Value));

            parser.Parse("-add", "1", "-add", "2", "-sub", "10", "-add", "4");

            Assert.AreEqual(-3, actual);
        }

        [TestMethod]
        public void RegisterContainerWithCollection()
        {
            var values = new List<string>();

            var parser = new Parser();

            parser.RegisterContainer<CollectionContainer>(
                b => b.With(c => c.Values.CurrentValue())
                    .Do(pc => values.Add(pc.Value)));

            var result = parser.Parse("-a", "1", "2", "3")
                .Result<CollectionContainer>();

            CollectionAssert.AreEquivalent(new[] {"1", "2", "3"}, result.Values.ToArray());
        }

        [TestMethod]
        public void RegisterContainerSameTypeTwice()
        {
            ExceptionAssert.Throws<ParserInitializationException>(() =>
                                                                  {
                                                                      var parser = new Parser();
                                                                      parser.RegisterContainer<TestContainer1>();
                                                                      parser.RegisterContainer<TestContainer1>();
                                                                  },
                ex => Assert.AreEqual(string.Format("Parser already knows a container of type {0}.", typeof (TestContainer1)), ex.Message));
        }

        [TestMethod]
        public void RegisterContainerSameInstanceTwice()
        {
            ExceptionAssert.Throws<ParserInitializationException>(
                () =>
                {
                    var parser = new Parser();
                    parser.RegisterContainer<TestContainer1>();
                    parser.RegisterContainer(new TestContainer1());
                },
                ex => Assert.AreEqual(string.Format("Parser already knows a container of type {0}.", typeof (TestContainer1)), ex.Message));
        }

        [TestMethod]
        public void RegisterContainerWithoutInstance()
        {
            var parser = new Parser();
            parser.RegisterContainer<TestContainer1>(null, null);

            var result = parser.Parse().Result<TestContainer1>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterOptionUnNamed()
        {
            var list = new List<int>();

            var parser = new Parser();

            parser.RegisterOption(b => b.Named("add")
                .Collection
                .As<int>()
                .Do(context => list.Add(context.Value)));

            parser.Parse("-add", "1", "2", "3");

            CollectionAssert.AreEquivalent(new[] {1, 2, 3}, list.ToArray());
        }

        [TestMethod]
        public void RegisterOptionCanParseKeyValuePairs()
        {
            var properties = new Dictionary<string, string>();

            var parser = new Parser();

            parser.RegisterOption(b => b.Named("p")
                .Collection
                .As<KeyValuePair<string, string>>()
                .Do(context => properties[context.Value.Key] = context.Value.Value));

            parser.Parse("/p:a=b", "/p:c=d", "/p:c=e");

            Assert.AreEqual(2, properties.Count);

            Assert.IsTrue(properties.ContainsKey("a"));
            Assert.IsTrue(properties.ContainsKey("c"));

            Assert.AreEqual("b", properties["a"]);
            Assert.AreEqual("e", properties["c"]);
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