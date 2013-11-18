using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Implementation.Reflection
{
    internal class CollectionPropertySetter : PropertySetter
    {
        private const string ReadOnlyCollectionNotInitializedMessage = 
            "The read only collection property '{0}' is not initialized.";

        private readonly IStringConverter _stringConverter;
        private readonly PropertyInfo _propertyInfo;
        private readonly object _instance;
        private MethodInfo _addMethod;
        private Type _itemType;
        private object _collectionInstance;

        public CollectionPropertySetter(IStringConverter stringConverter, PropertyInfo propertyInfo, object instance)
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

            Initialize();
        }

        private void Initialize()
        {
            Type collectionItemType = _propertyInfo.PropertyType.GetCollectionItemType();
            _itemType = collectionItemType;

            _collectionInstance = _propertyInfo.GetValue(_instance, null);
            if (_collectionInstance == null)
            {
                // try to initialize the uninitialized collection.

                if (!_propertyInfo.CanWrite)
                    throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, ReadOnlyCollectionNotInitializedMessage, _propertyInfo));

                Type collectionType = _propertyInfo.PropertyType;
                if (collectionType.GetConstructor(new Type[0]) == null)
                {
                    // if there is no parameterless constructor for the type, try to use new Collection<>()
                    collectionType = typeof (Collection<>).MakeGenericType(collectionItemType);
                }

                _collectionInstance = Activator.CreateInstance(collectionType);
                _propertyInfo.SetValue(_instance, _collectionInstance, null);
            }

            _addMethod = typeof (ICollection<>).MakeGenericType(collectionItemType).GetMethod("Add");
        }

        public override void SetValue(string value)
        {
            object realValue = _stringConverter.To(_itemType, value);

            _addMethod.Invoke(_collectionInstance, new[] {realValue});

            OnValueSet(new ValueSetEventArgs(_instance, _itemType, realValue));
        }

        public override Type ItemType { get { return _itemType; } }
    }
}