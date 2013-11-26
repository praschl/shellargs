using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using MiP.ShellArgs.ContainerAttributes;
using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Implementation.Reflection
{
    internal class PropertyReflector
    {
        private readonly IStringConverter _stringConverter;

        public PropertyReflector(IStringConverter stringConverter)
        {
            if (stringConverter == null)
                throw new ArgumentNullException("stringConverter");

            _stringConverter = stringConverter;
        }

        public ICollection<OptionDefinition> CreateOptionDefinitions(Type argumentType, object instance)
        {
            IEnumerable<OptionDefinition> optionDefinitions = argumentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                                          .Where(IsUsableProperty)
                                                                          .Select(pi => CreateOptionDefinition(pi, instance));

            return optionDefinitions.ToArray();
        }

        internal OptionDefinition CreateOptionDefinition(PropertyInfo propertyInfo, object instance)
        {
            Type itemType = propertyInfo.PropertyType;
            bool isCollection = itemType.IsOrImplementsICollection();

            itemType = itemType.GetCollectionItemType().MakeNotNullable();

            bool isBoolean = itemType == typeof (bool);

            IPropertySetter setter;
            if (isCollection)
                setter = new CollectionPropertySetter(_stringConverter, propertyInfo, instance);
            else if (isBoolean)
                setter = new BooleanPropertySetter(_stringConverter, propertyInfo, instance);
            else
                setter = new DefaultPropertySetter(_stringConverter, propertyInfo, instance);

            var aliases = new string[0];
            bool isRequired = false;
            int position = 0;

            string name = GetOptionName(propertyInfo);

            var aliasesAttribute = (AliasesAttribute)propertyInfo.GetCustomAttributes(typeof (AliasesAttribute), false).FirstOrDefault();
            if (aliasesAttribute != null)
                aliases = aliasesAttribute.Aliases;

            var requiredAttribute = (RequiredAttribute)propertyInfo.GetCustomAttributes(typeof (RequiredAttribute), false).FirstOrDefault();
            if (requiredAttribute != null)
                isRequired = true;

            var positionAttribute = (PositionAttribute)propertyInfo.GetCustomAttributes(typeof (PositionAttribute), false).FirstOrDefault();
            if (positionAttribute != null)
                position = positionAttribute.Position;

            string valueDescription = null;
            var optionDescriptionAttribute = (ValueDescriptionAttribute)propertyInfo.GetCustomAttributes(typeof (ValueDescriptionAttribute), false).FirstOrDefault();
            if (optionDescriptionAttribute != null)
                valueDescription = optionDescriptionAttribute.ValueDescription;

            return new OptionDefinition
                   {
                       Name = name,
                       Aliases = aliases,
                       Position = position,
                       IsRequired = isRequired,
                       IsBoolean = isBoolean,
                       IsCollection = isCollection,
                       ValueSetter = setter,
                       Description =
                       {
                           ValueDescription = valueDescription
                       }
                   };
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string GetOptionName(MemberInfo propertyInfo)
        {
            string name = propertyInfo.Name;

            var nameAttribute = (OptionAttribute)propertyInfo.GetCustomAttributes(typeof (OptionAttribute), false).FirstOrDefault();
            if (nameAttribute != null && !string.IsNullOrEmpty(nameAttribute.Name))
                name = nameAttribute.Name;

            return name;
        }

        private static bool IsUsableProperty(PropertyInfo p)
        {
            bool hasSetter = p.GetSetMethod(true) != null;
            bool requiresSetter = !p.PropertyType.IsOrImplementsICollection();
            bool hasGetter = p.GetGetMethod(true) != null;
            bool hasIgnoreAttribute = p.GetCustomAttributes(typeof (IgnoreOptionAttribute), false).Any();

            return (hasSetter || !requiresSetter)
                   && hasGetter
                   && !hasIgnoreAttribute;
        }
    }
}