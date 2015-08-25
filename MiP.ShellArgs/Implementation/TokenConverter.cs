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
        private Queue<string> _arguments;
        private OptionDefinition _lastOption;
        private string _lastCollectionOption;

        public TokenConverter(ArgumentFactory argumentFactory)
        {
            if (argumentFactory == null)
                throw new ArgumentNullException(nameof(argumentFactory));

            _argumentFactory = argumentFactory;
        }

        public IEnumerable<Token> ConvertToTokens(OptionContext optionContext, IEnumerable<string> arguments)
        {
            _arguments = new Queue<string>(arguments);

            return ParseTokens(optionContext)
                .SelectMany(tokens => tokens);
        }

        public IEnumerable<Token> ConvertToTokens(OptionContext optionContext, params string[] arguments)
        {
            _arguments = new Queue<string>(arguments);

            return ParseTokens(optionContext)
                .SelectMany(tokens => tokens);
        }

        private IEnumerable<IEnumerable<Token>> ParseTokens(OptionContext optionContext)
        {
            ICollection<OptionDefinition> optionDefinitions = optionContext.Definitions;

            Queue<OptionDefinition> positionalOptions = optionContext.Positionals;

            _lastOption = null;

            Token currentOptionToken = null;

            while (_arguments.Count > 0)
            {
                Argument parsedArgument = _argumentFactory.Parse(_arguments.Dequeue());

                if (parsedArgument.HasName)
                {
                    yield return FinalizePreviousOption(currentOptionToken);

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
                    yield return AddPair(currentOptionToken, Token.CreateValue(parsedArgument.Value));

                    // next value will require a new option unless the current option is a collection
                    if (!_lastOption.IsCollection)
                        _lastOption = null;
                }
            }

            yield return FinalizePreviousOption(currentOptionToken);
        }

        private IEnumerable<Token> FinalizePreviousOption(Token currentOptionToken)
        {
            if (_lastOption == null)
                return Enumerable.Empty<Token>();

            if (_lastOption.IsBoolean)
                return AddPair(currentOptionToken, Token.CreateValue(ToggleBoolean));

            if (!_lastOption.IsCollection)
                throw new ParsingException(string.Format(CultureInfo.InvariantCulture, NoValueAssignedMessage, _lastOption.Name));

            return Enumerable.Empty<Token>();
        }

        private IEnumerable<Token> AddPair(Token option, Token value)
        {
            if (!_lastOption.IsCollection)
                _lastCollectionOption = null;

            bool returnOption = _lastCollectionOption != option.Name;

            if (_lastOption.IsCollection)
                _lastCollectionOption = option.Name;

            if (returnOption)
                yield return option;

            yield return value;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void MapToContainer(IEnumerable<Token> tokens, OptionContext optionContext)
        {
            ICollection<OptionDefinition> optionDefinitions = optionContext.Definitions;

            var parsedOptions = new List<string>();

            IPropertySetter setter = null;

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

                    parsedOptions.Add(lastOptionName);
                }
            }

            string[] missingRequiredOptions = optionContext.Required.Select(o => o.Name).Except(parsedOptions).ToArray();

            if (missingRequiredOptions.Length > 0)
                throw new ParsingException(string.Format(CultureInfo.InvariantCulture, MissingRequiredOptionsMessage, string.Join(",", missingRequiredOptions)));
        }
    }
}