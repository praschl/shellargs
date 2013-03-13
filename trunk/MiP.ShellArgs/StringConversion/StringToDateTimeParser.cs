using System;
using System.Globalization;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to parse a string to a <see cref="DateTime"/>.
    /// </summary>
    public class StringToDateTimeParser : StringParser<DateTime>
    {
        /// <summary>
        /// Gets a text describing the intent of the value in help.
        /// </summary>
        public override string ValueDescription { get { return "date"; } }

        /// <summary>
        /// Parses the string to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The string to parse to <see cref="DateTime"/>.</param>
        /// <returns>An instance of <see cref="DateTime"/> which was parsed from <paramref name="value"/>.</returns>
        public override DateTime Parse(string value)
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}