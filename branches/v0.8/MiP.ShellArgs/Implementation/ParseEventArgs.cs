using System;

namespace MiP.ShellArgs.Implementation
{
    internal class ParseEventArgs : EventArgs
    {
        public ParseEventArgs(string optionName, object value)
        {
            OptionName = optionName;
            Value = value;
        }
        
        public string OptionName { get; private set; }

        public object Value { get; private set; }
    }
}