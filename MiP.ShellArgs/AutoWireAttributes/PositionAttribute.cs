using System;

namespace MiP.ShellArgs.AutoWireAttributes
{
    /// <summary>
    /// Used to specify a position for the option, making it a positional option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PositionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionAttribute"/> class.
        /// </summary>
        /// <param name="position">specifies the position of the option.</param>
        public PositionAttribute(int position)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the position for the option.
        /// </summary>
        public int Position { get; private set; }
    }
}