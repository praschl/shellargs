using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace MiP.ShellArgs.Implementation
{
    internal class TokenConverter
    {
        public const string ToggleBoolean = "MiP.ShellArgs.Tokens.ToggleBoolean";

        private const string ExpectedAnOptionMessage =
            "Expected an option instead of value '{0}'.";

        private const string NoValueAssignedMessage =
            "Option '{0}' has no value assigned.";

        private const string NotAValidOptionMessage =
            "'{0}' is not a valid option.";

        private const string MissingRequiredOptionsMessage =
            "The following option(s) are required, but were not given: [{0}].";

        private readonly ArgumentFactory _argumentFactory;
        private List<Token> _resultTokens;
        private Queue<string> _arguments;
        private OptionDefinition _lastOption;
        private string _lastCollectionOption;

        public TokenConverter(ArgumentFactory argumentFactory)
        {
            if (argumentFactory == null)
                throw new ArgumentNullException("argumentFactory");

            _argumentFactory = argumentFactory;
        }

        public IEnumerable<Token> ConvertToTokens(ICollection<OptionDefinition> optionDefinitions, IEnumerable<string> arguments)
        {
            _arguments = new Queue<string>(arguments);

            return ParseTokens(optionDefinitions);
        }

        public IEnumerable<Token> ConvertToTokens(ICollection<OptionDefinition> optionDefinitions, params string[] arguments)
        {
            _arguments = new Queue<string>(arguments);

            return ParseTokens(optionDefinitions);
        }

        private IEnumerable<Token> ParseTokens(ICollection<OptionDefinition> optionDefinitions)
        {
            IOrderedEnumerable<OptionDefinition> positionals = optionDefinitions.Where(o => o.IsPositional)
                                                                                .OrderBy(o => o.Position);

            var positionalOptions = new Queue<OptionDefinition>(positionals);

            _resultTokens = new List<Token>();
            _lastOption = null;

            Token currentOptionToken = null;

            while (_arguments.Count > 0)
            {
                Argument parsedArgument = _argumentFactory.Parse(_arguments.Dequeue());

                if (parsedArgument.HasName)
                {
                    FinalizePreviousOption(currentOptionToken);
                    string name = parsedArgument.Name;

                    OptionDefinition definition = optionDefinitions.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        ??
                        optionDefinitions.FirstOrDefault(o => o.Aliases != null && o.Aliases.Contains(name, StringComparer.OrdinalIgnoreCase));

                    if (definition == null)
                        throw new ParsingException(string.Format(CultureInfo.InvariantCulture, NotAValidOptionMessage, name));

                    name = definition.Name;

                    _lastOption = definition;

                    positionalOptions.Clear(); // when a name was given, positionals are no longer allowed -> deactivate the remaining.
                    currentOptionToken = Token.CreateOption(name);
                }

                if (_lastOption == null)
                {
                    if (!positionalOptions.Any())
                        throw new ParsingException(string.Format(CultureInfo.InvariantCulture, ExpectedAnOptionMessage, parsedArgument.Value));

                    _lastOption = positionalOptions.Dequeue();
                    currentOptionToken = Token.CreateOption(_lastOption.Name);
                }

                if (parsedArgument.HasValue)
                {
                    AddPair(currentOptionToken, Token.CreateValue(parsedArgument.Value));

                    // next value will require a new option unless the current option is a collection
                    if (!_lastOption.IsCollection)
                        _lastOption = null;
                }
            }

            FinalizePreviousOption(currentOptionToken);

            return _resultTokens;
        }

        private void FinalizePreviousOption(Token currentOptionToken)
        {
            if (_lastOption == null)
                return;

            if (_lastOption.IsBoolean)
            {
                AddPair(currentOptionToken, Token.CreateValue(ToggleBoolean));
                return;
            }

            if (!_lastOption.IsCollection)
                throw new ParsingException(string.Format(CultureInfo.InvariantCulture, NoValueAssignedMessage, _lastOption.Name));
        }

        private void AddPair(Token option, Token value)
        {
            if (!_lastOption.IsCollection)
                _lastCollectionOption = null;

            if (_lastCollectionOption != option.Name)
                _resultTokens.Add(option);

            _resultTokens.Add(value);

            if (_lastOption.IsCollection)
                _lastCollectionOption = option.Name;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void MapToContainer(IEnumerable<Token> tokens, ICollection<OptionDefinition> optionDefinitions)
        {
            IPropertySetter setter = null;
            List<string> requiredOptions = optionDefinitions.Where(o => o.IsRequired).Select(o => o.Name).ToList();

            string lastOptionName = null;
            foreach (Token currentToken in tokens)
            {
                if (currentToken.IsOption)
                {
                    OptionDefinition definition = optionDefinitions.FirstOrDefault(s => s.Name == currentToken.Name);
                    if (definition == null)
                        throw new ParsingException(string.Format(CultureInfo.InvariantCulture, NotAValidOptionMessage, currentToken.Name));

                    setter = definition.ValueSetter;
                    lastOptionName = definition.Name;
                }
                else
                {
                    if (setter == null)
                        throw new ParsingException(string.Format(CultureInfo.InvariantCulture, ExpectedAnOptionMessage, currentToken.Value));

                    setter.SetValue(currentToken.Value);

                    requiredOptions.Remove(lastOptionName);
                }
            }

            if (requiredOptions.Count > 0)
                throw new ParsingException(string.Format(CultureInfo.InvariantCulture, MissingRequiredOptionsMessage, string.Join(",", requiredOptions)));
        }
    }
}