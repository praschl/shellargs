using System;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to easily create implementations of <see cref="IStringParser" />.
    /// </summary>
    public abstract class StringParser : IStringParser
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
        public abstract bool CanParseTo(Type targetType);

        /// <summary>
        /// Determines whether the specified value is valid for the target type.
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is valid for the target type; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsValid(Type targetType, string value);

        /// <summary>
        /// Parses the string to &lt;TTarget&gt;
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The string to parse to &lt;TTarget&gt;.</param>
        /// <returns>
        /// An instance of &lt;TTarget&gt; which was parsed from <paramref name="value" />.
        /// </returns>
        public abstract object Parse(Type targetType, string value);
    }
}