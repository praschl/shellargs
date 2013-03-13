using System;

namespace MiP.ShellArgs.Implementation
{
    internal abstract class PropertySetter : IPropertySetter
    {
        public abstract void SetValue(string value);
        public abstract Type ItemType { get; }
        public event EventHandler<ValueSetEventArgs> ValueSet;
        
        protected void OnValueSet(ValueSetEventArgs e)
        {
            EventHandler<ValueSetEventArgs> temp = ValueSet;
            if (temp != null)
                temp(this, e);
        }
    }
}