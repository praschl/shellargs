using System;

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
        public virtual string ValueDescription { get { return null; } }

        /// <summary>
        /// Determines whether this instance can parse to the specified target type.
        /// </summary>
        /// <param name="targetType">Type to parse a string to.</param>
        /// <returns>
        ///   <c>true</c> if a string can be parsed to the specified target type; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanParseTo(Type targetType)
        {
            return targetType == typeof (TTarget);
        }

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