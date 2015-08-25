using System;
using System.Collections.Generic;
using System.Linq;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.Fluent
{
    internal class ContainerCustomizer<TContainer, TArgument> : IContainerCustomizer<TContainer, TArgument>
    {
        private readonly string _name;
        private readonly IParserBuilder _parser;
        private readonly ICollection<OptionDefinition> _optionDefinitions;
        private readonly IContainerBuilder<TContainer> _builder;

        public ContainerCustomizer(string name, IParserBuilder parser, ICollection<OptionDefinition> optionDefinitions, IContainerBuilder<TContainer> builder)
        {
            _name = name;
            _parser = parser;
            _optionDefinitions = optionDefinitions;
            _builder = builder;
        }

        public IContainerBuilder<TContainer> Do(Action<ParsingContext<TContainer, TArgument>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return Do(_name, handler);
        }

        private IContainerBuilder<TContainer> Do(string optionName, Action<ParsingContext<TContainer, TArgument>> handler)
        {
            if (string.IsNullOrEmpty(optionName))
                throw new ArgumentException("must not be null or empty", nameof(optionName));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            OptionDefinition foundDefinition = _optionDefinitions.FirstOrDefault(o => o.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
            if (foundDefinition != null)
                foundDefinition.ValueSetter.ValueSet += (o, e) =>
                                                        {
                                                            var context = new ParsingContext<TContainer, TArgument>(_parser, (TContainer)e.Instance, foundDefinition.Name, (TArgument)e.Value);
                                                            handler(context);
                                                        };

            return _builder;
        }
    }
}