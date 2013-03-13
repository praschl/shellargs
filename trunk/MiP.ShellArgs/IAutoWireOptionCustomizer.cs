using System;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to customize a option of a specific type.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container.</typeparam>
    /// <typeparam name="TArgument">The type of the option argument.</typeparam>
    public interface IAutoWireOptionCustomizer<TContainer, TArgument>
    {
        /// <summary>
        /// Used to specify the delegate to call when the option is parsed.
        /// </summary>
        /// <param name="handler">A handler which will be called when the option is parsed.</param>
        /// <returns>An untyped customizer used to customize other options.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Do"), SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IAutoWireOptionBuilder<TContainer> Do(Action<ParsingContext<TContainer, TArgument>> handler);
    }
}