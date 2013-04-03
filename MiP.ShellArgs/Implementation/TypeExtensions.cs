using System;
using System.Collections.Generic;
using System.Linq;

namespace MiP.ShellArgs.Implementation
{
    internal static class TypeExtensions
    {
        public static Type MakeNotNullable(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type innerType = Nullable.GetUnderlyingType(type);
            return innerType ?? type;
        }

        public static bool IsOrImplementsICollection(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            bool implementsICollection = type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ICollection<>));
            bool isICollection = type.IsGenericType && type.GetGenericTypeDefinition() == typeof (ICollection<>);

            return isICollection || implementsICollection;
        }

        public static Type GetCollectionItemType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsOrImplementsICollection())
                return type;

            // TODO: when type is a dictionary, return correct item type (KeyValuePair<,>)
            //var collectionType = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ICollection<>));

            return type.GetGenericArguments().First();
        }
    }
}