using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to parse strings to enums.
    /// </summary>
    public class StringToEnumParser : StringParser
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
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            return targetType.IsEnum;
        }

        /// <summary>
        /// Determines whether the specified value is valid for the target type.
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is valid for the target type; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override bool IsValid(Type targetType, string value)
        {
            return Enum.GetNames(targetType).Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
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
            // try type descriptor before Enum.Parse
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);
            if (converter.IsValid(value))
                return converter.ConvertFromInvariantString(value);

            // handle enums explicitly, because the type converter is not case insensitive
            return Enum.Parse(targetType, value, true);
        }
    }
}