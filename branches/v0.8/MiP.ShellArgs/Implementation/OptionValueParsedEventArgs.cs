using System;

namespace MiP.ShellArgs.Implementation
{
    /// <summary>
    /// Event args for when a value was parsed;
    /// </summary>
    public class OptionValueParsedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the parsed option.
        /// </summary>
        public string Option { get; private set; }

        /// <summary>
        /// Gets the parsed value of the option.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the parser builder.
        /// This allowes adding of options during the parse process.
        /// </summary>
        public IParserBuilder ParserBuilder { get; private set; }
        
        internal OptionValueParsedEventArgs(IParserBuilder parserBuilder, string option, object value)
        {
            ParserBuilder = parserBuilder;
            Option = option;
            Value = value;
        }
    }
}