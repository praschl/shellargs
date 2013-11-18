using System;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to add options and parsing behavior to the parser.
    /// </summary>
    public interface IParserBuilder
    {
        /// <summary>
        /// Used to add a class to the container and automatically create options wired to the properties of the class.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="container">An instance of the container.</param>
        /// <param name="builderDelegate">Used to customize the added container and its options.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void AutoWire<TContainer>(TContainer container, Action<IAutoWireOptionBuilder<TContainer>> builderDelegate) where TContainer : new();

        /// <summary>
        /// Adds a stand alone option to the parser and gives it a name.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <param name="builderDelegate">Used to customize the addded option.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        void WithOption(string name, Action<IOptionBuilder> builderDelegate);


        /// <summary>
        /// Adds a stand alone option to the parser.
        /// </summary>
        /// <param name="builderDelegate">Used to customize the addded option.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        void WithOption(Action<IOptionBuilder> builderDelegate);

        /// <summary>
        /// Used to add a callback which is called whenever an option was successfully parsed.
        /// </summary>
        /// <param name="handler">Called when any option is was successfully parsed.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void OnOptionParsed(Action<ParsingContext<object>> handler);
    }
}