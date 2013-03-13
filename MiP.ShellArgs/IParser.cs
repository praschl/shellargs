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
        /// Used to add a class to the container and automatically create options wired to the properties of the class.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="container">An instance of the container.</param>
        /// <param name="builderDelegate">Used to customize the added container and its options.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IParser AutoWire<TContainer>(TContainer container, Action<IAutoWireOptionBuilder<TContainer>> builderDelegate) where TContainer : new();

        /// <summary>
        /// Adds a stand alone option to the parser.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <param name="builderDelegate">Used to customize the addded option.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        IParser WithOption(string name, Action<IOptionBuilder> builderDelegate);

        /// <summary>
        /// Used to add a callback which is called whenever an option was successfully parsed.
        /// </summary>
        /// <param name="handler">Called when any option is was successfully parsed.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IParser OnOptionParsed(Action<ParsingContext<object>> handler);

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