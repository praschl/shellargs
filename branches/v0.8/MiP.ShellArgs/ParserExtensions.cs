using System;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Extends the <see cref="IParser"/> interface.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Used to add a class to the container and automatically create options wired to the properties of the class.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="parser">The parser to extend.</param>
        /// <param name="container">An instance of the container.</param>
        /// <returns>
        /// The current instance of <see cref="IParser" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">parser</exception>
        public static void RegisterContainer<TContainer>(this IParserBuilder parser, TContainer container) where TContainer : new()
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            parser.RegisterContainer(container, x => { });
        }

        /// <summary>
        /// Used to add a class to the container and automatically create options wired to the properties of the class.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="parser">The parser to extend.</param>
        /// <returns>
        /// The current instance of <see cref="IParser" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">parser</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void RegisterContainer<TContainer>(this IParserBuilder parser) where TContainer : new()
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            parser.RegisterContainer(new TContainer(), x => { });
        }

        /// <summary>
        /// Used to add a class to the container and automatically create options wired to the properties of the class.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container.</typeparam>
        /// <param name="parser">The parser to extend.</param>
        /// <param name="configurationCallback">The configuration callback.</param>
        /// <returns>
        /// The current instance of <see cref="IParser" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">parser</exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static void RegisterContainer<TContainer>(this IParserBuilder parser, Action<IContainerBuilder<TContainer>> configurationCallback) where TContainer : new()
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            parser.RegisterContainer(new TContainer(), configurationCallback);
        }
    }
}