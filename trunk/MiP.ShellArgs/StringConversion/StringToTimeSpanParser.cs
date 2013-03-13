using System;
using System.Globalization;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to parse a string to a <see cref="TimeSpan"/>.
    /// </summary>
    public class StringToTimeSpanParser : StringParser<TimeSpan>
    {
        /// <summary>
        /// Gets a text describing the intent of the value in help.
        /// </summary>
        public override string ValueDescription { get { return "time"; } }
      
        /// <summary>
        /// Parses the string to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The string to parse to <see cref="TimeSpan"/>.</param>
        /// <returns>An instance of <see cref="TimeSpan"/> which was parsed from <paramref name="value"/>.</returns>
        public override TimeSpan Parse(string value)
        {
            return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}