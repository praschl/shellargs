﻿using System;
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
        private Type _itemType;
        private object _collectionInstance;

        public CollectionPropertySetter(IStringConverter stringConverter, PropertyInfo propertyInfo, object instance)
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
        }

        public override void SetValue(string value)
        {
            object realValue = _stringConverter.To(_itemType, value);

            Add((dynamic)_collectionInstance, (dynamic)realValue);

            OnValueSet(new ValueSetEventArgs(_instance, _itemType, realValue));
        }

        private static void Add<T>(ICollection<T> collection, T value)
        {
            collection.Add(value);
        }

        private static void Add<TKey, TValue>(IDictionary<TKey, TValue> collection, KeyValuePair<TKey, TValue> value)
        {
            collection.Add(value);
        }

        public override Type ItemType => _itemType;
    }
}