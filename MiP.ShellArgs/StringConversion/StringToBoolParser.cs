using System;
using System.Globalization;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to parse a string to a <see cref="bool"/>.
    /// </summary>
    public class StringToBoolParser : StringParser<bool>
    {
        /// <summary>
        /// Gets a text describing the intent of the value in help.
        /// </summary>
        public override string ValueDescription { get { return "bool"; } }

        /// <summary>
        /// Parses the string to <see cref="bool"/>. Also supports "+" for <c>true</c> and "-" for <c>false</c>.
        /// </summary>
        /// <param name="value">The string to parse to <see cref="bool"/>.</param>
        /// <returns>An instance of <see cref="bool"/> which was parsed from <paramref name="value"/>.</returns>
        public override bool Parse(string value)
        {
            if (value == "+")
                return true;
            if (value == "-")
                return false;

            return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
        }
    }
}