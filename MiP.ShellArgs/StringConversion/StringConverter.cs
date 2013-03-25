using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.StringConversion
{
    internal class StringConverter : IStringConverter, IStringParserProvider
    {
        private const string CouldNotParseValueToTypeMessage = "Could not parse value '{0}' to type {1}.";

        private readonly IDictionary<Type, IStringParser> _parsers;

        private static readonly IDictionary<Type, IStringParser> _defaultParsers = new Dictionary<Type, IStringParser>
                                                                          {
                                                                              {typeof (bool), new StringToBoolParser()},
                                                                              {typeof (DateTime), new StringToDateTimeParser()},
                                                                              {typeof (TimeSpan), new StringToTimeSpanParser()}
                                                                          };

        public StringConverter(IDictionary<Type, IStringParser> parsers)
        {
            _parsers = parsers ?? _defaultParsers;
        }

        public object To(Type targetType, string value)
        {
            try
            {
                // when the target is nullable<T> just get the T
                targetType = targetType.MakeNotNullable();

                IStringParser correctParser = GetParser(targetType);

                if (correctParser != null)
                    return correctParser.Parse(value);

                // handle enums
                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, true);
                
                // try type descriptor
                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter.IsValid(value))
                    return converter.ConvertFromInvariantString(value);

                // if no method worked, just Convert
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture); // last chance
            }
            catch (Exception ex)
            {
                throw new ParsingException(string.Format(CultureInfo.InvariantCulture, CouldNotParseValueToTypeMessage, value, targetType.AssemblyQualifiedName), ex);
            }
        }
        
        public IStringParser GetParser(Type targetType)
        {
            IStringParser correctParser;
            
            _parsers.TryGetValue(targetType, out correctParser);

            return correctParser;
        }
    }
}