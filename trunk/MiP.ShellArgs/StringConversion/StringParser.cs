namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to easily create implementations of <see cref="IStringParser" /> and <see cref="IStringParser{TTarget}" />
    /// </summary>
    /// <typeparam name="TTarget">The type of the target type.</typeparam>
    public abstract class StringParser<TTarget> : IStringParser<TTarget>
    {
        /// <summary>
        /// Gets a text describing the intent of the value in help.
        /// </summary>
        public abstract string ValueDescription { get; }

        /// <summary>
        /// Parses the string to &lt;TTarget&gt;
        /// </summary>
        /// <param name="value">The string to parse to &lt;TTarget&gt;.</param>
        /// <returns>An instance of &lt;TTarget&gt; which was parsed from <paramref name="value"/>.</returns>
        public abstract TTarget Parse(string value);

        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>
        /// An object which was parsed from <paramref name="value" />.
        /// </returns>
        object IStringParser.Parse(string value)
        {
            return Parse(value);
        }
    }
}