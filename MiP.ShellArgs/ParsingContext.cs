namespace MiP.ShellArgs
{
    /// <summary>
    /// Passed to handler methods when an option is parsed.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    public class ParsingContext<TArgument>
    {
        internal ParsingContext(IParser parser, string option, TArgument value)
        {
            Parser = parser;
            Option = option;
            Value = value;
        }

        internal IParser Parser { get; private set; }

        /// <summary>
        /// Gets the name of the option which was parsed.
        /// </summary>
        public string Option { get; private set; }

        /// <summary>
        /// Gets the value which was parsed.
        /// </summary>
        public TArgument Value { get; private set; }
    }

    /// <summary>
    /// Passed to handler methods when an option of a registered container is parsed.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container.</typeparam>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    public class ParsingContext<TContainer, TArgument> : ParsingContext<TArgument>
    {
        internal ParsingContext(IParser parser, TContainer container, string option, TArgument value)
            : base(parser, option, value)
        {
            Container = container;
        }

        /// <summary>
        /// Gets the instance of the container.
        /// </summary>
        public TContainer Container { get; private set; }
    }
}