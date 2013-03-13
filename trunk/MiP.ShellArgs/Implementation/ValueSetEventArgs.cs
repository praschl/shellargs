using System;

namespace MiP.ShellArgs.Implementation
{
    internal class ValueSetEventArgs : EventArgs
    {
        public object Instance { get; private set; }
        public Type ValueType { get; private set; }
        public object Value { get; private set; }

        public ValueSetEventArgs(Type valueType, object value)
            : this(null, valueType, value)
        {
        }

        public ValueSetEventArgs(object instance, Type valueType, object value)
        {
            Instance = instance;
            ValueType = valueType;
            Value = value;
        }
    }
}