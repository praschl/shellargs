using System;
using System.Diagnostics.CodeAnalysis;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Fluent
{
    internal class OptionBuilder : IOptionBuilder
    {
        private const string ActionsOfICollectionNotSupportedMessage =
            "Actions of type ICollection<T> are not supported, use ParseAsCollection<T> instead.";

        private readonly IParser _parser;
        private readonly OptionDefinition _optionDefinition;
        private readonly IStringConverter _stringConverter;

        internal OptionBuilder(IParser parser, OptionDefinition optionDefinition, IStringConverter stringConverter)
        {
            _parser = parser;
            _optionDefinition = optionDefinition;
            _stringConverter = stringConverter;
        }
        
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ICollection", Justification = "Design time message")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ParseAsCollection", Justification = "Design time message")]
        public IOptionBuilder<TArgument> As<TArgument>()
        {
            if (typeof (TArgument).IsOrImplementsICollection())
                throw new NotSupportedException(ActionsOfICollectionNotSupportedMessage);

            return new OptionBuilder<TArgument>(_parser, _optionDefinition, _stringConverter);
        }

        public IOptionBuilder Collection
        {
            get
            {
                _optionDefinition.IsCollection = true;
                return this;
            }
        }

        public IOptionBuilder AtPosition(int position)
        {
            _optionDefinition.Position = position;
            return this;
        }

        public IOptionBuilder Required()
        {
            _optionDefinition.IsRequired = true;
            return this;
        }

        public IOptionBuilder ValueDescription(string description)
        {
            _optionDefinition.Description.ValueDescription = description;
            return this;
        }

        public IOptionBuilder Alias(params string[] aliases)
        {
            _optionDefinition.Aliases = aliases ?? new string[0];
            return this;
        }

        public IOptionBuilder Named(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name must not be null or empty.");
            
            _optionDefinition.Name = name;
            return this;
        }
    }

    internal class OptionBuilder<TArgument> : IOptionBuilder<TArgument>
    {
        private readonly IParser _parser;
        private readonly OptionDefinition _optionDefinition;
        private readonly IStringConverter _stringConverter;

        internal OptionBuilder(IParser parser, OptionDefinition optionDefinition, IStringConverter stringConverter)
        {
            _parser = parser;
            _optionDefinition = optionDefinition;
            _stringConverter = stringConverter;

            Type itemType = typeof (TArgument).MakeNotNullable();
            bool isBool = itemType == typeof (bool);
            _optionDefinition.IsBoolean = isBool;
        }

        public void Do(Action<ParsingContext<TArgument>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _optionDefinition.ValueSetter = new DelegatingPropertySetter<TArgument>(_stringConverter, value => callback(new ParsingContext<TArgument>(_parser, _optionDefinition.Name, value)));
        }
    }
}