using System.Collections.Generic;

namespace MiP.ShellArgs.Implementation
{
    internal class OptionContext
    {
        // TODO: this class needs testing...

        private readonly List<OptionDefinition> _definitions = new List<OptionDefinition>();
        private readonly List<OptionDefinition> _required = new List<OptionDefinition>();
        private readonly Queue<OptionDefinition> _positionals = new Queue<OptionDefinition>();

        private readonly OptionValidator _validator = new OptionValidator();

        public delegate void OptionAddedHandler(OptionDefinition definition);

        public event OptionAddedHandler OptionAdded;

        public ICollection<OptionDefinition> Definitions => _definitions;

        public ICollection<OptionDefinition> Required => _required;

        public Queue<OptionDefinition> Positionals => _positionals;

        public void Add(OptionDefinition definition)
        {
            _definitions.Add(definition);

            AddSpecial(definition);

            _validator.Validate(_definitions);
        }

        public void AddRange(ICollection<OptionDefinition> definitions)
        {
            _definitions.AddRange(definitions);

            foreach (OptionDefinition definition in definitions)
            {
                AddSpecial(definition);
            }

            _validator.Validate(_definitions);
        }

        private void AddSpecial(OptionDefinition definition)
        {
            InvokeOptionAdded(definition);

            if (definition.IsPositional)
                _positionals.Enqueue(definition);

            if (definition.IsRequired)
                _required.Add(definition);
        }

        private void InvokeOptionAdded(OptionDefinition definition)
        {
            OptionAddedHandler temp = OptionAdded;
            temp?.Invoke(definition);
        }
    }
}