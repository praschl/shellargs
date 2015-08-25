using System;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Implementation
{
    internal class DelegatingPropertySetter<TItem> : PropertySetter
    {
        private readonly IStringConverter _stringConverter;
        private readonly Action<TItem> _setter;

        public DelegatingPropertySetter(IStringConverter stringConverter, Action<TItem> setter)
        {
            if (stringConverter == null)
                throw new ArgumentNullException(nameof(stringConverter));
            if (setter == null)
                throw new ArgumentNullException(nameof(setter));

            _stringConverter = stringConverter;
            _setter = setter;
        }

        public override void SetValue(string value)
        {
            object realValue = _stringConverter.To(typeof (TItem), value);

            _setter((TItem)realValue);

            OnValueSet(new ValueSetEventArgs(typeof (TItem), realValue));
        }

        public override Type ItemType => typeof (TItem);
    }
}