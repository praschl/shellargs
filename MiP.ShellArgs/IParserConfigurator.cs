using System.Diagnostics.CodeAnalysis;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to specify which <see cref="IStringParser"/> to use to parse a string to <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TTarget">The target type to parse strings to.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configurator")]
    public interface IParserConfigurator<in TTarget>
    {
        /// <summary>
        /// Specifies that <typeparamref name="TParser"/> should be used to parse strings to <typeparamref name="TTarget"/>.
        /// </summary>
        /// <typeparam name="TParser">The type of the parser.</typeparam>
        /// <returns>The current instance of <see cref="IParserConfigurator{TTarget}"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "With")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void With<TParser>() where TParser : IStringParser, new();

        /// <summary>
        /// Specifies that <paramref name="parser"/> should be used to parse strings to <typeparamref name="TTarget"/>.
        /// </summary>
        /// <typeparam name="TParser">The type of the parser.</typeparam>
        /// <param name="parser">An instance of the parser to use.</param>
        /// <returns>The current instance of <see cref="IParserConfigurator{TTarget}"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "With")]
        void With<TParser>(TParser parser) where TParser : IStringParser;
    }
}