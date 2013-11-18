using System;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to convert a string to a typed value.
    /// </summary>
    public interface IStringConverter
    {
        /// <summary>
        /// Tries to convert <paramref name="value"/> to the <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The type to convert the <paramref name="value"/> to.</param>
        /// <param name="value">The value to convert to the <paramref name="targetType"/>.</param>
        /// <returns>An instance of the targetType.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "To")]
        object To(Type targetType, string value);
    }
}