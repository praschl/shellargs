using System;

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
        /// Determines whether this instance can parse to the specified target type.
        /// </summary>
        /// <param name="targetType">Type to parse a string to.</param>
        /// <returns>
        ///   <c>true</c> if a string can be parsed to the specified target type; otherwise, <c>false</c>.
        /// </returns>
        bool CanParseTo(Type targetType);

        /// <summary>
        /// Determines whether the specified value is valid for the target type.
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is valid for the target type; otherwise, <c>false</c>.
        /// </returns>
        bool IsValid(Type targetType, string value);

        /// <summary>
        /// Parses the string to the <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        /// An object which was parsed from <paramref name="value" /> to <paramref name="targetType"/>.
        /// </returns>
        object Parse(Type targetType, string value);
    }
}