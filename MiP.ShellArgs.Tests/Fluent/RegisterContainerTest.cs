using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FakeItEasy;

using FluentAssertions;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var parser = A.Fake<IParserBuilder>();

            var stringConverter = new StringConverter(new ParserSettings().ParserProvider);
            var reflector = new PropertyReflector(stringConverter);
            
            _builder = new ContainerBuilder<TestContainer>(container, parser, reflector);
        }

        [TestMethod]
        public void CreatesCorrectOptions()
        {
            OptionDefinition[] definitions = _builder.OptionDefinitions.ToArray();

            OptionDefinition[] expected = 
                {
                new OptionDefinition {Name = "AString"},
                new OptionDefinition {Name = "ANumber"},
                new OptionDefinition {Name = "Collection"}
            };

            definitions.ShouldAllBeEquivalentTo(expected, o => o.Including(x => x.Name));
        }

        [TestMethod]
        public void SubscribesToEvent()
        {
            ParsingContext<TestContainer, string> stringContainer = null;

            _builder.With(x => x.AString).Do(pc => stringContainer = pc);

            _builder.OptionDefinitions.First(x => x.Name == "AString").ValueSetter.SetValue("Hurray");

            stringContainer.Should().NotBeNull();
            stringContainer.Option.Should().Be("AString");
            stringContainer.Value.Should().Be("Hurray");
            stringContainer.Container.AString.Should().Be("Hurray");
        }

        [TestMethod]
        public void SubscribesToEventWhenNamed()
        {
            ParsingContext<TestContainer, string> stringContainer = null;

            _builder.With<string>("AString").Do(pc => stringContainer = pc);

            _builder.OptionDefinitions.First(x => x.Name == "AString").ValueSetter.SetValue("Hurray");

            stringContainer.Should().NotBeNull();
            stringContainer.Option.Should().Be("AString");
            stringContainer.Value.Should().Be("Hurray");
            stringContainer.Container.AString.Should().Be("Hurray");
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

            values.ShouldAllBeEquivalentTo(new[] { "Hello", "World" });
        }

        [TestMethod]
        public void RegisterOptionDoesNotAcceptPropertyOfCollection()
        {
            Action with = () => _builder.With(c => c.Collection.Count);

            with.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void RegisterOptionDoesNotAcceptPropertyOfProperty()
        {
            Action with = () => _builder.With(c => c.Child.AString);

            with.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void RegisterOptionCollectionDoesNotAcceptAnythingButCurrentValue()
        {
            Action with = () => _builder.With(c => c.Collection.Last());

            with.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void RegisterOptionCollectionDoesNotAcceptAccessToExternalInstances()
        {
            var cont = new TestContainer();

            Action with = () => _builder.With(c => cont.Collection.CurrentValue());

            with.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void RegisterOptionThrowsWhenOptionIsUnknown()
        {
            Action with = () => _builder.With<string>("nooption");

            with.ShouldThrow<ParserInitializationException>()
                .WithMessage($"The container {typeof(TestContainer)} does not provide the option '{"nooption"}'.");
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