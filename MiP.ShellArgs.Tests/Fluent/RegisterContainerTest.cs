using System;
using System.Collections.Generic;
using System.Linq;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;
using MiP.ShellArgs.Tests.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MiP.ShellArgs.Tests.Fluent
{
    [TestClass]
    public class RegisterContainerTest
    {
        private ContainerBuilder<TestContainer> _builder;
        
        [TestInitialize]
        public void Initialize()
        {
            var container = new TestContainer();

            var parserMock = new Mock<IParserBuilder>();

            var stringConverter = new StringConverter(new ParserSettings().ParserProvider);
            var reflector = new PropertyReflector(stringConverter);
            
            _builder = new ContainerBuilder<TestContainer>(container, parserMock.Object, reflector);
        }

        [TestMethod]
        public void CreatesCorrectOptions()
        {
            OptionDefinition[] definitions = _builder.OptionDefinitions.ToArray();

            Assert.AreEqual(3, definitions.Length);
            Assert.AreEqual("AString", definitions[0].Name);
            Assert.AreEqual("ANumber", definitions[1].Name);
        }

        [TestMethod]
        public void SubscribesToEvent()
        {
            ParsingContext<TestContainer, string> stringContainer = null;

            _builder.With(x => x.AString).Do(pc => stringContainer = pc);

            _builder.OptionDefinitions.First(x => x.Name == "AString").ValueSetter.SetValue("Hurray");

            Assert.IsNotNull(stringContainer);
            Assert.AreEqual("AString", stringContainer.Option);
            Assert.AreEqual("Hurray", stringContainer.Value);
            Assert.AreEqual("Hurray", stringContainer.Container.AString);
        }

        [TestMethod]
        public void SubscribesToEventWhenNamed()
        {
            ParsingContext<TestContainer, string> stringContainer = null;

            _builder.With<string>("AString").Do(pc => stringContainer = pc);

            _builder.OptionDefinitions.First(x => x.Name == "AString").ValueSetter.SetValue("Hurray");

            Assert.IsNotNull(stringContainer);
            Assert.AreEqual("AString", stringContainer.Option);
            Assert.AreEqual("Hurray", stringContainer.Value);
            Assert.AreEqual("Hurray", stringContainer.Container.AString);
        }

        [TestMethod]
        public void RegisterOptionAcceptsExtensionMethodCurrentValue()
        {
            var values = new List<string>();

            _builder.With(c => c.Collection.CurrentValue())
                    .Do(pc => values.Add(pc.Value));

            IPropertySetter setter = _builder.OptionDefinitions.First(d => d.Name == "Collection").ValueSetter;
            setter.SetValue("Hello");
            setter.SetValue("World");

            CollectionAssert.AreEqual(new[] {"Hello", "World"}, values.ToArray());
        }

        [TestMethod]
        public void RegisterOptionDoesNotAcceptPropertyOfCollection()
        {
            ExceptionAssert.Throws<NotSupportedException>(
                () => _builder.With(c => c.Collection.Count), Assert.IsNotNull);
        }

        [TestMethod]
        public void RegisterOptionDoesNotAcceptPropertyOfProperty()
        {
            ExceptionAssert.Throws<NotSupportedException>(
                () => _builder.With(c => c.Child.AString), Assert.IsNotNull);
        }

        [TestMethod]
        public void RegisterOptionCollectionDoesNotAcceptAnythingButCurrentValue()
        {
            ExceptionAssert.Throws<NotSupportedException>(
                () => _builder.With(c => c.Collection.Last()), Assert.IsNotNull);
        }

        [TestMethod]
        public void RegisterOptionCollectionDoesNotAcceptAccessToExternalInstances()
        {
            var cont = new TestContainer();

            ExceptionAssert.Throws<NotSupportedException>(
                () => _builder.With(c => cont.Collection.CurrentValue()), Assert.IsNotNull);
        }

        [TestMethod]
        public void RegisterOptionThrowsWhenOptionIsUnknown()
        {
            ExceptionAssert.Throws<ParserInitializationException>(() => _builder.With<string>("nooption"),
                ex => Assert.AreEqual(string.Format("The container {0} does not provide the option '{1}'.", typeof (TestContainer), "nooption"), ex.Message));
        }

        public class TestContainer
        {
            public string AString { get; set; }
            public int ANumber { get; set; }

            public List<string> Collection { get; set; }

            [IgnoreOption]
            public TestContainer Child { get; set; }
        }
    }
}