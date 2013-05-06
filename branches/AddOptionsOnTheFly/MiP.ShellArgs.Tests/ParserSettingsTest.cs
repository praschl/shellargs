using System.Linq;

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

            CollectionAssert.AreEquivalent(_settings.Assignments.ToArray(), new[] {'a', 'b'});
        }

        [TestMethod]
        public void PrefixWithWorks()
        {
            _settings.PrefixWith('a', 'b');

            CollectionAssert.AreEquivalent(_settings.Prefixes.ToArray(), new[] {"a", "b"});
        }

        [TestMethod]
        public void EnableShortBooleansWorks()
        {
            _settings.EnableShortBooleans(false);

            Assert.IsFalse(_settings.ShortBooleansEnabled);
            Assert.AreEqual(0, _settings.ShortBooleans.Length);

            _settings.EnableShortBooleans(true);

            Assert.IsTrue(_settings.ShortBooleansEnabled);
            CollectionAssert.AreEquivalent(new[] {"+", "-"}, _settings.ShortBooleans);
        }
        
    }
}