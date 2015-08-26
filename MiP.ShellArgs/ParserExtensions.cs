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
        public static IContainerBuilder<TContainer> RegisterContainer<TContainer>(this IParserBuilder parser, TContainer container) where TContainer : new()
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            return parser.RegisterContainer(container);
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
        public static IContainerBuilder<TContainer> RegisterContainer<TContainer>(this IParserBuilder parser) where TContainer : new()
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            return parser.RegisterContainer(new TContainer());
        }
    }
}