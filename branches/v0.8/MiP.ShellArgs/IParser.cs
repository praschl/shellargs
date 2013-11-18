using System;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to customize a parser and parse shell arguments.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Customizes the parser by changing default settings.
        /// </summary>
        /// <param name="customizer">An action which changes the <see cref="ParserSettings"/>.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        IParser Customize(Action<ParserSettings> customizer);

        /// <summary>
        /// Gets a short help description of the options.
        /// </summary>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetShortHelp();

        /// <summary>
        /// Parses the specified args. Values are set on the properties of the containers or passed to the callback handlers.
        /// </summary>
        /// <param name="args">The shell args to parse.</param>
        /// <returns>An instance of <see cref="IParserResult"/> which contains the result of the operation.</returns>
        IParserResult Parse(params string[] args);
    }
}