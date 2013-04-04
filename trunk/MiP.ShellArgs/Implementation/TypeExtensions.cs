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

        private static bool IsICollection(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        private static Type GetICollectionType(Type type)
        {
            return type.GetInterfaces().FirstOrDefault(IsICollection);
        }

        private static bool ImplementsICollection(Type type)
        {
            return GetICollectionType(type) != null;
        }

        public static bool IsOrImplementsICollection(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return ImplementsICollection(type) || IsICollection(type);
        }

        public static Type GetCollectionItemType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!IsICollection(type))
                type = (GetICollectionType(type) ?? type);

            if (!IsICollection(type))
                return type;

            return type.GetGenericArguments().First();
        }
    }
}