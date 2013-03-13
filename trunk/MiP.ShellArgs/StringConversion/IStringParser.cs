namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to parse a string.
    /// </summary>
    public interface IStringParser
    {
        /// <summary>
        /// Gets a text describing the intent of the value in help.
        /// </summary>
        string ValueDescription { get; }

        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>An object which was parsed from <paramref name="value"/>.</returns>
        object Parse(string value);
    }

    /// <summary>
    /// Used to parse a string to &lt;TTarget&gt;
    /// </summary>
    /// <typeparam name="TTarget">The type to parse the string to.</typeparam>
    public interface IStringParser<out TTarget> : IStringParser
    {
        /// <summary>
        /// Parses the string to &lt;TTarget&gt;
        /// </summary>
        /// <param name="value">The string to parse to &lt;TTarget&gt;.</param>
        /// <returns>An instance of &lt;TTarget&gt; which was parsed from <paramref name="value"/>.</returns>
        new TTarget Parse(string value);
    }
}