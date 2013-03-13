using System;
using System.Reflection;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Implementation.Reflection
{
    internal class DefaultPropertySetter : PropertySetter
    {
        private readonly IStringConverter _stringConverter;
        private readonly PropertyInfo _propertyInfo;
        private readonly object _instance;
        private readonly Type _type;

        public DefaultPropertySetter(IStringConverter stringConverter, PropertyInfo propertyInfo, object instance)
        {
            if (stringConverter == null)
                throw new ArgumentNullException("stringConverter");
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (instance == null)
                throw new ArgumentNullException("instance");

            _stringConverter = stringConverter;
            _propertyInfo = propertyInfo;
            _instance = instance;

            _type = _propertyInfo.PropertyType;
        }

        public override void SetValue(string value)
        {
            object realValue = _stringConverter.To(_type, value);

            _propertyInfo.SetValue(_instance, realValue, null);

            OnValueSet(new ValueSetEventArgs(_instance, _type, realValue));
        }

        public override Type ItemType { get { return _type; } }
    }
}