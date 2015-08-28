using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiP.ShellArgs.Tests
{
    [TestClass]
    public class ParserSettingsTest
    {
        private ParserSettings _settings;

        [TestInitialize]
        public void Initialize()
        {
            _settings = new ParserSettings();
        }

        [TestMethod]
        public void AssignWithWorks()
        {
            _settings.AssignWith('a', 'b');

            _settings.Assignments.ShouldAllBeEquivalentTo(new[] {'a', 'b'});
        }

        [TestMethod]
        public void PrefixWithWorks()
        {
            _settings.PrefixWith('a', 'b');

            _settings.Prefixes.ShouldAllBeEquivalentTo(new[] {"a", "b"});
        }

        [TestMethod]
        public void EnableShortBooleansWorks()
        {
            _settings.EnableShortBooleans(false);

            _settings.ShortBooleansEnabled.Should().BeFalse();
            _settings.ShortBooleans.Length.Should().Be(0);

            _settings.EnableShortBooleans(true);

            _settings.ShortBooleansEnabled.Should().BeTrue();
            _settings.ShortBooleans.ShouldAllBeEquivalentTo(new[] {"+", "-"});
        }
        
    }
}