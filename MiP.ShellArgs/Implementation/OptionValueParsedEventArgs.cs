using System;

namespace MiP.ShellArgs.Implementation
{
    /// <summary>
    /// Event args for when a value was parsed;
    /// </summary>
    public class OptionValueParsedEventArgs : EventArgs
    {
        internal OptionValueParsedEventArgs(ParsingContext<object> parsingContext)
        {
            ParsingContext = parsingContext;
        }

        /// <summary>
        /// Gets the parsing context.
        /// </summary>
        public ParsingContext<object> ParsingContext { get; private set; }
    }
}