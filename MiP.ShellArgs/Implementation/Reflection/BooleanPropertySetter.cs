using System;
using System.Reflection;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Implementation.Reflection
{
    internal class BooleanPropertySetter : PropertySetter
    {
        private readonly IStringConverter _stringConverter;
        private readonly PropertyInfo _propertyInfo;
        private readonly object _instance;

        public BooleanPropertySetter(IStringConverter stringConverter, PropertyInfo propertyInfo, object instance)
        {
            if (stringConverter == null)
                throw new ArgumentNullException(nameof(stringConverter));
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _stringConverter = stringConverter;
            _propertyInfo = propertyInfo;
            _instance = instance;
        }

        public override void SetValue(string value)
        {
            object realValue = GetRealValue(value);

            _propertyInfo.SetValue(_instance, realValue, null);

            OnValueSet(new ValueSetEventArgs(_instance, typeof (bool), realValue));
        }

        public override Type ItemType => typeof (bool);

        private object GetRealValue(string value)
        {
            object realValue;
            if (value == TokenConverter.ToggleBoolean)
            {
                var currentValue = (bool?)_propertyInfo.GetValue(_instance, null);
                realValue = currentValue != true;
            }
            else
            {
                realValue = _stringConverter.To(typeof (bool), value);
            }
            return realValue;
        }
    }
}