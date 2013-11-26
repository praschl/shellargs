using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Extends <see cref="ICollection{T}"/> with a method that is used to specify the current parsed option value.
    /// </summary>
    public static class OptionCollectionExtensions
    {
        /// <summary>
        /// Used to specify the item in a call to <see cref="IContainerBuilder{TContainer}.RegisterOption{TArgument}(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The extended collection.</param>
        /// <returns></returns>
        /// <remarks>
        /// This function always throws a <see cref="NotImplementedException"/>. This method should only ever be used in expressions 
        /// to determine an option property without having to specify the property by name.
        /// </remarks>
        /// <exception cref="NotImplementedException">always.</exception>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "collection")]
        [ExcludeFromCodeCoverage]
        public static T CurrentValue<T>(this ICollection<T> collection)
        {
            throw new NotImplementedException("This method should only ever be used in expressions to determine an option property without having to specify the property by name.");
        }
    }
}