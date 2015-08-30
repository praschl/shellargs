using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to get the results of parsing.
    /// </summary>
    public interface IParserResult
    {
        /// <summary>
        /// Returns an instance of <typeparamref name="TContainer"/> which properties were set while parsing.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container to get.</typeparam>
        /// <returns>An instance of <typeparamref name="TContainer"/>.</returns>
        TContainer Result<TContainer>();

        /// <summary>
        /// Puts the results to <paramref name="result"/>. This overload is useful when you use more than one container.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container to get.</typeparam>
        /// <param name="result">Will hold the container instance..</param>
        /// <returns>The current instance of <see cref="IParserResult"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Overload is available")]
        IParserResult ResultTo<TContainer>(out TContainer result);
    }
}