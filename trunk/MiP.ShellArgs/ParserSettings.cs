using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using MiP.ShellArgs.Fluent;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to customize the parser.
    /// </summary>
    public class ParserSettings
    {
        private const string AllowAtLeastOnePrefixMessage =
            "Allow at least one prefix for options or you would not be able to use any options.";

        private static readonly string[] _defaultShortBooleans = new[] {"+", "-"};

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class.
        /// </summary>
        public ParserSettings()
        {
            Prefixes = new[] {"-", "/"};
            Assignments = new[] {'=', ':'};
            ShortBooleans = _defaultShortBooleans;
            ShortBooleansEnabled = true;
            StringParsers = new Dictionary<Type, IStringParser>();
        }

        internal bool ShortBooleansEnabled { get; set; }

        internal string[] Prefixes { get; set; }

        internal char[] Assignments { get; set; }

        internal string[] ShortBooleans { get; private set; }

        internal IDictionary<Type, IStringParser> StringParsers { get; private set; }

        /// <summary>
        /// Enables or disable the short booleans.
        /// </summary>
        /// <remarks>
        /// Short boolean values are enabled by default.
        /// </remarks>
        /// <param name="allow">if set to <c>true</c> allows short boolean syntax.</param>
        /// <returns>The current instance of <see cref="ParserSettings"/>.</returns>
        public ParserSettings EnableShortBooleans(bool allow)
        {
            ShortBooleansEnabled = allow;
            ShortBooleans = allow ? _defaultShortBooleans : new string[0];

            return this;
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
        public ParserSettings PrefixWith(params char[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException(AllowAtLeastOnePrefixMessage, "prefixes");

            Prefixes = prefixes.Distinct().Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            return this;
        }

        /// <summary>
        /// Used to set the characters which are used to create an assignment.
        /// </summary>
        /// <remarks>
        /// Default assignment characters are '=' and ':'.
        /// </remarks>
        /// <param name="assignmentOperators">The assignment operators the parser should use.</param>
        /// <returns>The current instance of <see cref="ParserSettings"/>.</returns>
        public ParserSettings AssignWith(params char[] assignmentOperators)
        {
            // assignment with empty array is allowed
            if (assignmentOperators == null)
                assignmentOperators = new char[0];

            Assignments = assignmentOperators;
            return this;
        }

        internal ParserSettings RegisterStringParser<TParser, TTarget>(TParser parser) where TParser : IStringParser<TTarget>
        {
            StringParsers.Add(typeof (TTarget), parser);

            return this;
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