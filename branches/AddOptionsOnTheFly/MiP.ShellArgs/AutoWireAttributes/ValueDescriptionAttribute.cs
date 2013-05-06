using System;

namespace MiP.ShellArgs.AutoWireAttributes
{
    /// <summary>
    /// Used to specify a description for an option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ValueDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="valueDescription">
        /// Describes the intent of the value.
        /// </param>
        public ValueDescriptionAttribute(string valueDescription)
        {
            ValueDescription = valueDescription;
        }

        /// <summary>
        /// Gets or sets the description for an option.
        /// </summary>
        public string ValueDescription { get; private set; }
    }
}