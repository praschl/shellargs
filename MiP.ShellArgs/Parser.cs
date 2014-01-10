using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to parse shell arguments into pocos or call event handlers when parsed.
    /// </summary>
    public class Parser : IParser, IParserBuilder
    {
        private const string ParserAlreadyKnowsContainerMessage = "Parser already knows a container of type {0}.";

        private readonly Dictionary<string, object> _containerByOption = new Dictionary<string, object>();
        private readonly Dictionary<Type, object> _containerByType = new Dictionary<Type, object>();
        private readonly OptionContext _optionContext = new OptionContext();
        private readonly ParserSettings _settings;
        
        private readonly StringConverter _stringConverter;
        private readonly PropertyReflector _propertyReflector;
        private readonly TokenConverter _converter;
        private readonly HelpGenerator _generator;

        /// <summary>
        /// Occurs when a value of an option was successfully parsed.
        /// </summary>
        public event EventHandler<OptionValueParsedEventArgs> OptionValueParsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        public Parser()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="parserSettings">Configures the parser.</param>
        public Parser(ParserSettings parserSettings)
        {
            _optionContext.OptionAdded += definition =>
                                          {
                                              if (definition.ValueSetter != null)
                                                  definition.ValueSetter.ValueSet += (o, e) => OnParse(new OptionValueParsedEventArgs(this, definition.Name, e.Value));
                                          };

            _settings = parserSettings ?? new ParserSettings();

            _stringConverter = new StringConverter(_settings.ParserProvider);
            _propertyReflector = new PropertyReflector(_stringConverter);
            _converter = new TokenConverter(new ArgumentFactory(_settings));
            _generator = new HelpGenerator(_settings.ParserProvider);
        }

        /// <summary>
        /// Used to add a class to the container and automatically create options wired to the properties of the class.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="container">An instance of the container.</param>
        /// <param name="builderDelegate">Used to customize the added container and its options.</param>
        /// <returns>
        /// The current instance of <see cref="IParser" />.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public void RegisterContainer<TContainer>(TContainer container, Action<IContainerBuilder<TContainer>> builderDelegate) where TContainer : new()
        {
            if (ReferenceEquals(container, null))
                container = new TContainer();

            // the important stuff is done by autowiring, its ok to pass a null for the delegate
            if (builderDelegate == null)
                builderDelegate = x => { };

            if (_containerByType.ContainsKey(typeof (TContainer)))
                throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, ParserAlreadyKnowsContainerMessage, typeof (TContainer)));

            _containerByType.Add(typeof (TContainer), container);
            var builder = new ContainerBuilder<TContainer>(container, this, _propertyReflector);

            builderDelegate(builder);

            OptionDefinition[] newDefinitions = builder.OptionDefinitions.ToArray();

            foreach (OptionDefinition currentDefinition in newDefinitions)
            {
                _containerByOption.Add(currentDefinition.Name, container);
            }

            _optionContext.AddRange(newDefinitions);
        }

        /// <summary>
        /// Adds a stand alone option to the parser.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <param name="builderDelegate">Used to customize the addded option.</param>
        /// <returns>
        /// The current instance of <see cref="IParser" />.
        /// </returns>
        /// <exception cref="System.ArgumentException">Parameter name must not be null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">builderDelegate</exception>
        public void RegisterOption(string name, Action<IOptionBuilder> builderDelegate)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name must not be null or empty.", "name");

            if (builderDelegate == null)
                throw new ArgumentNullException("builderDelegate");

            RegisterOption(b =>
                           {
                               b.Named(name);
                               builderDelegate(b);
                           });
        }

        /// <summary>
        /// Adds a stand alone option to the parser.
        /// </summary>
        /// <param name="builderDelegate">Used to customize the addded option.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        public void RegisterOption(Action<IOptionBuilder> builderDelegate)
        {
            if (builderDelegate == null)
                throw new ArgumentNullException("builderDelegate");

            var newDefinition = new OptionDefinition();

            var builder = new OptionBuilder(this, newDefinition, _stringConverter);

            builderDelegate(builder);

            _optionContext.Add(newDefinition);
        }
        
        /// <summary>
        /// Parses the specified args. Values are set on the properties of the containers or passed to the callback handlers.
        /// </summary>
        /// <param name="args">The shell args to parse.</param>
        /// <returns>
        /// An instance of <see cref="IParserResult" /> which contains the result of the operation.
        /// </returns>
        public IParserResult Parse(params string[] args)
        {
            if (args == null)
                args = new string[0];

            IEnumerable<Token> tokens = _converter.ConvertToTokens(_optionContext, args);

            _converter.MapToContainer(tokens, _optionContext);

            return new ParserResult(_containerByType);
        }

        #region Super simple static Parse()

        /// <summary>
        /// Parses the specified args into an instance of <typeparamref name="TContainer"/>.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="args">The args to parse.</param>
        /// <returns>The container with the parsed values.</returns>
        public static TContainer Parse<TContainer>(params string[] args) where TContainer : new()
        {
            var parser = new Parser();
            parser.RegisterContainer<TContainer>();

            return parser.Parse(args)
                .Result<TContainer>();
        }

        /// <summary>
        /// Parses the specified args into an instance of <typeparamref name="TContainer"/> using the specified settings.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="settings">Used to override default settings.</param>
        /// <param name="args">The args to parse.</param>
        /// <returns>The container with the parsed values.</returns>
        public static TContainer Parse<TContainer>(ParserSettings settings, params string[] args) where TContainer : new()
        {
            var parser = new Parser(settings);
            parser.RegisterContainer<TContainer>();

            return parser.Parse(args)
                .Result<TContainer>();
        }

        /// <summary>
        /// Parses the specified args into the <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="container">The container to parse the values to.</param>
        /// <param name="args">The args to parse.</param>
        /// <returns>The container with the parsed values.</returns>
        public static void Parse<TContainer>(TContainer container, params string[] args) where TContainer : new()
        {
            var parser = new Parser();
            parser.RegisterContainer(container);
            parser.Parse(args);
        }

        /// <summary>
        /// Parses the specified args into the <paramref name="container"/> using the specified settings.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="container">The container to parse the values to.</param>
        /// <param name="settings">Used to override default settings.</param>
        /// <param name="args">The args to parse.</param>
        /// <returns>The container with the parsed values.</returns>
        public static void Parse<TContainer>(TContainer container, ParserSettings settings, params string[] args) where TContainer : new()
        {
            var parser = new Parser(settings);
            parser.RegisterContainer(container);
            parser.Parse(args);
        }

        #endregion

        /// <summary>
        /// Gets a short help description of the options.
        /// </summary>
        /// <returns>
        /// The current instance of <see cref="IParser" />.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetShortHelp()
        {
            _generator.OptionPrefix = _settings.Prefixes.First()[0];

            return _generator.GetParameterHelp(_optionContext.Definitions);
        }

        private void OnParse(OptionValueParsedEventArgs e)
        {
            EventHandler<OptionValueParsedEventArgs> raiseMe = OptionValueParsed;
            if (raiseMe != null)
                raiseMe(this, e);
        }
    }

    public static class ParserBuilderExtensions
    {
        public static void RegisterOption<T>(this IParserBuilder parserBuilder, string name, string[] aliases = null, int position = 0, bool collection = false, bool required = false, string valueDescription = null, Action<ParsingContext<T>> callback = null)
        {
            if (parserBuilder == null)
                throw new ArgumentNullException("parserBuilder");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name must not be null or empty.", "name");
            if (callback == null)
                throw new ArgumentNullException("callback");

            parserBuilder.RegisterOption(builder =>
                                         {
                                             builder.Named(name);
                                             builder.Alias(aliases);

                                             if (position > 0)
                                                 builder.AtPosition(position);
                                             if (collection)
                                                 builder = builder.Collection;

                                             builder.Required();
                                             builder.ValueDescription(valueDescription);

                                             builder.As<T>().Do(callback);
                                         });
        }

        public static void RegisterOption<T>(this IParserBuilder parserBuilder, Option<T> option)
        {
            if (parserBuilder == null)
                throw new ArgumentNullException("parserBuilder");
            if (option == null)
                throw new ArgumentNullException("option");
            if (option.Callback == null)
                throw new InvalidOperationException("Parameter option does not have a callback. This makes the option useless.");

            if (string.IsNullOrEmpty(option.Name))
                throw new InvalidOperationException("Parameter option must have a name.");

            parserBuilder.RegisterOption(builder =>
                                         {
                                             builder.Named(option.Name);
                                             builder.Alias(option.Aliases);

                                             if (option.Position > 0)
                                                 builder.AtPosition(option.Position);
                                             if (option.IsCollection)
                                                 builder = builder.Collection;

                                             builder.Required();
                                             builder.ValueDescription(option.ValueDescription);

                                             builder.As<T>().Do(option.Callback);
                                         });
        }
    }

    public class Option<T>
    {
        public string Name { get; set; }
        public string[] Aliases { get; set; }
        public int Position { get; set; }
        public bool IsCollection { get; set; }
        public Action<ParsingContext<T>> Callback { get; set; }
        public string ValueDescription { get; set; }
    }
}