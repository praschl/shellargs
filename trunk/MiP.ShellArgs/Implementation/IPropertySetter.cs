using System;

namespace MiP.ShellArgs.Implementation
{
    internal interface IPropertySetter
    {
        void SetValue(string value);

        Type ItemType { get; }

        event EventHandler<ValueSetEventArgs> ValueSet;
    }
}