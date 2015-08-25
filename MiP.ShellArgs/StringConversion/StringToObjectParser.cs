using System;
using System.ComponentModel;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used as a parser which tries to parse any string to the given type.
    /// </summary>
    public class StringToObjectParser : StringParser
    {
        /// <summary>
        /// Determines whether this instance can parse to the specified target type.
        /// </summary>
        /// <param name="targetType">Type to parse a string to.</param>
        /// <returns>
        ///   <c>true</c> if a string can be parsed to the specified target type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanParseTo(Type targetType)
        {
            return true;
        }

        /// <summary>
        /// Determines whether the specified value is valid for the target type.
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is valid for the target type; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid(Type targetType, string value)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            return converter.IsValid(value);
        }

        /// <summary>
        /// Parses the string to &lt;TTarget&gt;
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The string to parse to &lt;TTarget&gt;.</param>
        /// <returns>
        /// An instance of &lt;TTarget&gt; which was parsed from <paramref name="value" />.
        /// </returns>
        public override object Parse(Type targetType, string value)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            return converter.ConvertFromInvariantString(value);
        }
    }
}