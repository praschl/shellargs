using System;
using System.Diagnostics.CodeAnalysis;

namespace MiP.ShellArgs.AutoWireAttributes
{
    /// <summary>
    /// Used to specify aliases for an option.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AliasesAttribute : Attribute
    {
        private string[] _aliases;

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasesAttribute"/> class.
        /// </summary>
        public AliasesAttribute()
        {
            _aliases = new string[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasesAttribute"/> class.
        /// </summary>
        /// <param name="aliases">Aliases for the option.</param>
        public AliasesAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }

        /// <summary>
        /// Gets or sets the aliases for an option.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public string[] Aliases { get { return _aliases; } set { _aliases = value ?? new string[0]; } }
    }
}