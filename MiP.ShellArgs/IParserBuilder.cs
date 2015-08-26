using System;
using System.Diagnostics.CodeAnalysis;

using MiP.ShellArgs.Implementation;

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
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IContainerBuilder<TContainer> RegisterContainer<TContainer>(TContainer container) where TContainer : new();

        /// <summary>
        /// Adds a stand alone option to the parser and gives it a name.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        IOptionBuilder RegisterOption(string name);

        /// <summary>
        /// Adds a stand alone option to the parser.
        /// </summary>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        IOptionBuilder RegisterOption();

        /// <summary>
        /// Occurs when a value of an option was successfully parsed.
        /// </summary>
        event EventHandler<OptionValueParsedEventArgs> OptionValueParsed;
    }
}