using System;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to specify an option.
    /// </summary>
    public interface IOptionBuilder
    {
        /// <summary>
        /// Specifies the name of the option.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>An instance of <see cref="IOptionBuilder{TArgument}"/>.</returns>
        IOptionBuilder Named(string name);

        /// <summary>
        /// Specifies the result type of the option argument.
        /// </summary>
        /// <typeparam name="TArgument">The type of the option argument.</typeparam>
        /// <returns>An instance of <see cref="IOptionBuilder{TArgument}"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As")]
        IOptionBuilder<TArgument> As<TArgument>();

        /// <summary>
        /// Specifies that this option allowes more than one value.
        /// </summary>
        IOptionBuilder Collection { get; }

        /// <summary>
        /// Specifies the position of the option making it a positional option.
        /// </summary>
        /// <param name="position">The position at which this option may be used without a name.</param>
        /// <returns>The current instance of <see cref="IOptionBuilder"/>.</returns>
        IOptionBuilder AtPosition(int position);

        /// <summary>
        /// Specifies that the option is required and must not be omitted.
        /// </summary>
        /// <returns>The current instance of <see cref="IOptionBuilder"/>.</returns>
        IOptionBuilder Required();

        /// <summary>
        /// Specifies aliases (shortcuts) for the option.
        /// </summary>
        /// <param name="aliases">The aliases which may be used instead of the complete option name.</param>
        /// <returns>The current instance of <see cref="IOptionBuilder"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Alias")]
        IOptionBuilder Alias(params string[] aliases);

        /// <summary>
        /// Specifies a text which describes the intent of the options argument.
        /// </summary>
        /// <param name="description">Describes the intent of the options argument.</param>
        /// <returns>The current instance of <see cref="IOptionBuilder"/>.</returns>
        IOptionBuilder ValueDescription(string description);
    }

    /// <summary>
    /// Used to complete defining an option.
    /// </summary>
    /// <typeparam name="TArgument">The type of the argument.</typeparam>
    public interface IOptionBuilder<TArgument>
    {
        /// <summary>
        /// Used to specify a callback which is called when the option is parsed.
        /// </summary>
        /// <param name="callback">Called when the option is parsed.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Do")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void Do(Action<ParsingContext<TArgument>> callback);
    }
}