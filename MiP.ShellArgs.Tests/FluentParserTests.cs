using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MiP.ShellArgs.AutoWireAttributes;
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
            parser.AutoWire<RequiredAndNonRequiredOption>();

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
            parser.AutoWire<RequiredAndNonRequiredOption>(c => c.WithOption(x => x.NonRequired).Do(eventsRaised.Add));
            parser.Parse("-r:V1", "-n:V2");

            Assert.AreEqual(1, eventsRaised.Count);
            Assert.AreEqual("V2", eventsRaised[0].Value);
        }

        [TestMethod]
        public void EventIsRaisedForRenamedOption()
        {
            var eventsRaised = new List<ParsingContext<RenamedOption, string>>();

            var parser = new Parser();
            parser.AutoWire<RenamedOption>(c => c.WithOption(x => x.OriginalName).Do(eventsRaised.Add));
            parser.Parse("-newName:Value");

            Assert.AreEqual(1, eventsRaised.Count);
            Assert.AreEqual("Value", eventsRaised[0].Value);
        }

        [TestMethod]
        public void OverriddenPrefixesAreUsed()
        {
            var eventsRaised = new List<ParsingContext<object>>();

            var parser = new Parser();
            parser.Customize(c => c.PrefixWith('+'));
            parser.WithOption("Hello", b => b.As<string>().Do(x => { }));
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

            var parser = new Parser();
            parser.Customize(c => c.AssignWith('+'));
            parser.WithOption("Hello", b => b.As<string>().Do(x => { }));
            parser.OnOptionParsed(eventsRaised.Add);

            parser.Parse("-Hello+World");

            Assert.AreEqual(1, eventsRaised.Count);
            Assert.AreEqual("Hello", eventsRaised[0].Option);
            Assert.AreEqual("World", eventsRaised[0].Value);
        }

        [TestMethod]
        public void ShortHelpIsGenerated()
        {
            var parser = new Parser();
            parser.Customize(c => c.PrefixWith('+'));
            parser.WithOption("Hello", b => b.As<string>().Do(x => { }));

            string help = parser.GetShortHelp();
            Assert.AreEqual("[+Hello string]", help);
        }

        [TestMethod]
        public void ShortHelpIsGeneratedWithRenamedOption()
        {
            var parser = new Parser();
            parser.Customize(c => c.PrefixWith('+'));
            parser.WithOption("Hello", b => b.ValueDescription("something").As<string>().Do(x => { }));

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

            parser.AutoWire<TestContainer1>(
                aw => aw.WithOption(c => c.AString1).Do(pc => stringContext1 = pc)
                    .WithOption(c => c.ANumber1).Do(pc => intContext1 = pc));

            parser.AutoWire<TestContainer2>(
                aw => aw.WithOption<string>("AString2").Do(pc => stringContext2 = pc)
                    .WithOption<int>("ANumber2").Do(pc => intContext2 = pc));

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
        public void TwoWithOptions()
        {
            int actual = 0;

            var parser = new Parser();

            parser.WithOption("add",
                b => b.As<int>()
                    .Do(pc => actual += pc.Value));

            parser.WithOption("sub",
                b => b.As<int>()
                    .Do(pc => actual -= pc.Value));

            parser.Parse("-add", "1", "-add", "2", "-sub", "10", "-add", "4");

            Assert.AreEqual(-3, actual);
        }

        [TestMethod]
        public void AutoWireWithOptionCollection()
        {
            var values = new List<string>();

            var parser = new Parser();

            parser.AutoWire<CollectionContainer>(
                b => b.WithOption(c => c.Values.CurrentValue())
                    .Do(pc => values.Add(pc.Value)));

            var result = parser.Parse("-a", "1", "2", "3")
                .Result<CollectionContainer>();

            CollectionAssert.AreEquivalent(new[] {"1", "2", "3"}, result.Values.ToArray());
        }

        [TestMethod]
        public void AutoWireAContainerTwice()
        {
            ExceptionAssert.Throws<ParserInitializationException>(() =>
                                                                  {
                                                                      var parser = new Parser();
                                                                      parser.AutoWire<TestContainer1>();
                                                                      parser.AutoWire<TestContainer1>();
                                                                  },
                ex => Assert.AreEqual(string.Format("Parser already knows a container of type {0}.", typeof (TestContainer1)), ex.Message));
        }

        [TestMethod]
        public void AutoWireAContainerTwiceInstance()
        {
            ExceptionAssert.Throws<ParserInitializationException>(
                () =>
                {
                    var parser = new Parser();
                    parser.AutoWire<TestContainer1>();
                    parser.AutoWire(new TestContainer1());
                },
                ex => Assert.AreEqual(string.Format("Parser already knows a container of type {0}.", typeof (TestContainer1)), ex.Message));
        }

        [TestMethod]
        public void AutoWireWithoutInstance()
        {
            var parser = new Parser();
            parser.AutoWire<TestContainer1>(null, null);

            var result = parser.Parse().Result<TestContainer1>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WithOptionUnNamed()
        {
            var list = new List<int>();

            var parser = new Parser();

            parser.WithOption(b => b.Named("add")
                .Collection
                .As<int>()
                .Do(context => list.Add(context.Value)));

            parser.Parse("-add", "1", "2", "3");

            CollectionAssert.AreEquivalent(new[] {1, 2, 3}, list.ToArray());
        }

        [TestMethod]
        public void WithOptionCanParseKeyValuePairs()
        {
            var properties = new Dictionary<string, string>();

            var parser = new Parser();

            parser.WithOption(b => b.Named("p")
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