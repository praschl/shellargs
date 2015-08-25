using System.Collections.Generic;
using System.Globalization;

namespace MiP.ShellArgs.Implementation
{
    internal class OptionDefinition
    {
        public OptionDefinition()
        {
            Aliases = new string[0];
            Description = new OptionDescription();
        }

        public string Name { get; set; }

        public ICollection<string> Aliases { get; set; }

        public int Position { get; set; }

        public bool IsPositional => Position > 0;

        public bool IsRequired { get; set; }

        public bool IsBoolean { get; set; }

        public bool IsCollection { get; set; }

        public OptionDescription Description { get; private set; }

        public IPropertySetter ValueSetter { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Name: {0}, Position: {1}, IsRequired: {2}, IsBoolean: {3}, IsCollection: {4}", Name, Position, IsRequired, IsBoolean, IsCollection);
        }
    }
}