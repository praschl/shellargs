using System;

namespace MiP.ShellArgs.AutoWireAttributes
{
    /// <summary>
    /// Used to set the name of the option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public OptionAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of an option.
        /// </summary>
        public string Name { get; private set; }
    }
}