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
        /// <param name="builderDelegate">Used to customize the added container and its options.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void RegisterContainer<TContainer>(TContainer container, Action<IContainerBuilder<TContainer>> builderDelegate) where TContainer : new();
        
        /// <summary>
        /// Adds a stand alone option to the parser and gives it a name.
        /// </summary>
        /// <param name="name">Name of the option.</param>
        /// <param name="builderDelegate">Used to customize the addded option.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        void RegisterOption(string name, Action<IOptionBuilder> builderDelegate);
        
        /// <summary>
        /// Adds a stand alone option to the parser.
        /// </summary>
        /// <param name="builderDelegate">Used to customize the addded option.</param>
        /// <returns>The current instance of <see cref="IParser"/>.</returns>
        void RegisterOption(Action<IOptionBuilder> builderDelegate);

        /// <summary>
        /// Occurs when a value of an option was successfully parsed.
        /// </summary>
        event EventHandler<OptionValueParsedEventArgs> OptionValueParsed;
    }
}