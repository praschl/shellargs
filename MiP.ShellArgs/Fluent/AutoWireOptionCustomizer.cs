﻿using System;
using System.Collections.Generic;
using System.Linq;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.Fluent
{
    internal class AutoWireOptionCustomizer<TContainer, TArgument> : IAutoWireOptionCustomizer<TContainer, TArgument>
    {
        private readonly string _name;
        private readonly IParser _parser;
        private readonly ICollection<OptionDefinition> _optionDefinitions;
        private readonly IAutoWireOptionBuilder<TContainer> _builder;

        public AutoWireOptionCustomizer(string name, IParser parser, ICollection<OptionDefinition> optionDefinitions, IAutoWireOptionBuilder<TContainer> builder)
        {
            _name = name;
            _parser = parser;
            _optionDefinitions = optionDefinitions;
            _builder = builder;
        }

        public IAutoWireOptionBuilder<TContainer> Do(Action<ParsingContext<TContainer, TArgument>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return Do(_name, handler);
        }

        private IAutoWireOptionBuilder<TContainer> Do(string optionName, Action<ParsingContext<TContainer, TArgument>> handler)
        {
            if (string.IsNullOrEmpty(optionName))
                throw new ArgumentException("must not be null or empty", "optionName");

            if (handler == null)
                throw new ArgumentNullException("handler");

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