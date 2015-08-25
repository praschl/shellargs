using System;
using System.Globalization;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to parse a string to a <see cref="bool"/>.
    /// </summary>
    public class StringToBoolParser : StringParser
    {
        /// <summary>
        /// Gets a text describing the intent of the value in help.
        /// </summary>
        public override string ValueDescription => "bool";

        /// <summary>
        /// Determines whether this instance can parse to the specified target type.
        /// </summary>
        /// <param name="targetType">Type to parse a string to.</param>
        /// <returns>
        ///   <c>true</c> if a string can be parsed to the specified target type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanParseTo(Type targetType)
        {
            return targetType == typeof (bool);
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
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            switch (value.ToUpperInvariant())
            {
                case "+":
                case "-":
                case "TRUE":
                case "FALSE":
                    return true;

                default:
                    return false;
            }
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
            if (value == "+")
                return true;
            if (value == "-")
                return false;

            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }
    }
}