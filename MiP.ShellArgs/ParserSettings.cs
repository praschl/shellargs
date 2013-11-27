using System;
using System.Globalization;
using System.Linq;

using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs
{
    // TODO: make ParserSettings immutable / freeze while parsing, maybe even directly after adding it to parser.
    // throw an exception when it should be changed while parsing.

    /// <summary>
    /// Used to customize the parser.
    /// </summary>
    public class ParserSettings
    {
        private const string AllowAtLeastOnePrefixMessage =
            "Allow at least one prefix for options or you would not be able to use any options.";

        private static readonly string[] _defaultShortBooleans = {"+", "-"};

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class.
        /// </summary>
        public ParserSettings()
        {
            Prefixes = new[] {"-", "/"};
            Assignments = new[] {'=', ':'};
            ShortBooleans = _defaultShortBooleans;
            ShortBooleansEnabled = true;
            ParserProvider = new StringParserProvider(this);
        }

        internal bool ShortBooleansEnabled { get; set; }

        internal string[] Prefixes { get; set; }

        internal char[] Assignments { get; set; }

        internal string[] ShortBooleans { get; private set; }

        internal IStringParserProvider ParserProvider { get; private set; }

        /// <summary>
        /// Enables or disable the short booleans.
        /// </summary>
        /// <remarks>
        /// Short boolean values are enabled by default.
        /// </remarks>
        /// <param name="allow">if set to <c>true</c> allows short boolean syntax.</param>
        /// <returns>The current instance of <see cref="ParserSettings"/>.</returns>
        public void EnableShortBooleans(bool allow)
        {
            ShortBooleansEnabled = allow;
            ShortBooleans = allow ? _defaultShortBooleans : new string[0];
        }

        /// <summary>
        /// Used to set the characters which prefix an option.
        /// </summary>
        /// <remarks>
        /// Default prefixes are '/' and '-'.
        /// </remarks>
        /// <param name="prefixes">The prefixes the parser should use.</param>
        /// <returns>The current instance of <see cref="ParserSettings"/>.</returns>
        /// <exception cref="System.ArgumentException">prefixes</exception>
        public void PrefixWith(params char[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException(AllowAtLeastOnePrefixMessage, "prefixes");

            Prefixes = prefixes.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
        }

        /// <summary>
        /// Used to set the characters which are used to create an assignment.
        /// </summary>
        /// <remarks>
        /// Default assignment characters are '=' and ':'.
        /// </remarks>
        /// <param name="assignmentOperators">The assignment operators the parser should use.</param>
        /// <returns>The current instance of <see cref="ParserSettings"/>.</returns>
        public void AssignWith(params char[] assignmentOperators)
        {
            // assignment with empty array is allowed
            if (assignmentOperators == null)
                assignmentOperators = new char[0];

            Assignments = assignmentOperators;
        }

        internal void RegisterStringParser<TParser>(TParser parser) where TParser : IStringParser
        {
            ParserProvider.RegisterParser(parser);
        }

        /// <summary>
        /// Used to specify that <typeparamref name="TTarget"/> is known to the parser and can be parsed to.
        /// </summary>
        /// <remarks>
        /// To specify the parser which should be used, continue with the 
        /// <see cref="IParserConfigurator{TTarget}.With{TParser}()"/> or
        /// <see cref="IParserConfigurator{TTarget}.With{TParser}(TParser)"/> methods.
        /// </remarks>
        /// <typeparam name="TTarget">Specifies the type the parser knows how to parse.</typeparam>
        /// <returns>An instance of <see cref="IParserConfigurator{TTarget}"/> which is used to specify the string parser.</returns>
        public IParserConfigurator<TTarget> ParseTo<TTarget>()
        {
            return new ParserConfigurator<TTarget>(this);
        }
    }
}