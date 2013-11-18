using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.StringConversion
{
    /// <summary>
    /// Used to parse a string into a <see cref="KeyValuePair{TKey,TValue}"/>; This parser expects the type of the key to be <see cref="string"/>.
    /// </summary>
    public class StringToKeyValuePairParser : StringParser
    {
        // TODO: performance: build some caching of parsing and reflection

        private readonly IStringParserProvider _stringParserProvider;
        private readonly ParserSettings _settings;

        internal StringToKeyValuePairParser(IStringParserProvider stringParserProvider, ParserSettings settings)
        {
            if (stringParserProvider == null)
                throw new ArgumentNullException("stringParserProvider");
            if (settings == null)
                throw new ArgumentNullException("settings");

            _stringParserProvider = stringParserProvider;
            _settings = settings;
        }

        /// <summary>
        /// Determines whether this instance can parse to the specified target type.
        /// </summary>
        /// <param name="targetType">Type to parse a string to.</param>
        /// <returns>
        ///   <c>true</c> if a string can be parsed to the specified target type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanParseTo(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            if (!targetType.IsGenericType)
                return false;

            if (targetType.GetGenericTypeDefinition() != typeof (KeyValuePair<,>))
                return false;

            Type keyType = targetType.GetGenericArguments()[0];
            if (keyType != typeof (string))
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified value is valid for the target type.
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is valid for the target type; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid(Type targetType, string value)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            if (value == null)
                throw new ArgumentNullException("value");

            Type valueType = targetType.GetGenericArguments()[1].MakeNotNullable();

            IStringParser parser = _stringParserProvider.GetParser(valueType);

            int index = value.IndexOfAny(_settings.Assignments);
            if (index < 1)
                return false;

            string right = value.Substring(index + 1);

            return parser.IsValid(valueType, right);
        }

        /// <summary>
        /// Parses the string to &lt;TTarget&gt;
        /// </summary>
        /// <param name="targetType">Type to convert to.</param>
        /// <param name="value">The string to parse to &lt;TTarget&gt;.</param>
        /// <returns>
        /// An instance of &lt;TTarget&gt; which was parsed from <paramref name="value" />.
        /// </returns>
        public override object Parse(Type targetType, string value)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            if (value == null)
                throw new ArgumentNullException("value");

            Type valueType = targetType.GetGenericArguments()[1].MakeNotNullable();

            IStringParser parser = _stringParserProvider.GetParser(valueType);

            int index = value.IndexOfAny(_settings.Assignments);
            if (index < 1)
                return false;

            string left = value.Substring(0, index);
            string right = value.Substring(index + 1);

            object parsedValue = parser.Parse(valueType, right);

            return CreateKeyValuePair(valueType, left, parsedValue);
        }

        private static object CreateKeyValuePair(Type valueType, string left, object value)
        {
            Type pairType = typeof (KeyValuePair<,>)
                .MakeGenericType(typeof (string), valueType);

            ConstructorInfo constructor = pairType.GetConstructor(new[]
                                                                  {
                                                                      typeof (string),
                                                                      valueType
                                                                  });

            if (constructor == null)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Expected to find a constructor of types [string, {0}].", valueType));

            return constructor.Invoke(new[] {left, value});
        }
    }
}