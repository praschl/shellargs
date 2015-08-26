using System;
using System.Diagnostics.CodeAnalysis;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Fluent
{
    internal class OptionBuilder : IOptionBuilder
    {
        // TODO: find correct text for ParseAsCollection<T> and replace by nameof()
        private const string ActionsOfICollectionNotSupportedMessage =
            "Actions of type ICollection<T> are not supported, use ParseAsCollection<T> instead.";

        private readonly IParserBuilder _parser;
        private readonly OptionDefinition _optionDefinition;
        private readonly IStringConverter _stringConverter;
        private readonly OptionContext _optionContext;

        internal OptionBuilder(IParserBuilder parser, OptionDefinition optionDefinition, IStringConverter stringConverter, OptionContext optionContext)
        {
            _parser = parser;
            _optionDefinition = optionDefinition;
            _stringConverter = stringConverter;
            _optionContext = optionContext;
        }
        
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ICollection", Justification = "Design time message")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ParseAsCollection", Justification = "Design time message")]
        public IOptionBuilder<TArgument> As<TArgument>()
        {
            if (typeof (TArgument).IsOrImplementsICollection())
                throw new NotSupportedException(ActionsOfICollectionNotSupportedMessage);

            return new OptionBuilder<TArgument>(_parser, _optionDefinition, _stringConverter, _optionContext);
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
        private readonly IParserBuilder _parser;
        private readonly OptionDefinition _optionDefinition;
        private readonly IStringConverter _stringConverter;
        private readonly OptionContext _optionContext;

        internal OptionBuilder(IParserBuilder parser, OptionDefinition optionDefinition, IStringConverter stringConverter, OptionContext optionContext)
        {
            _parser = parser;
            _optionDefinition = optionDefinition;
            _stringConverter = stringConverter;
            _optionContext = optionContext;

            Type itemType = typeof (TArgument).MakeNotNullable();
            bool isBool = itemType == typeof (bool);
            _optionDefinition.IsBoolean = isBool;
        }

        public void Do(Action<ParsingContext<TArgument>> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _optionDefinition.ValueSetter = new DelegatingPropertySetter<TArgument>(_stringConverter, value => callback(new ParsingContext<TArgument>(_parser, _optionDefinition.Name, value)));

            _optionContext.Add(_optionDefinition);
        }
    }
}