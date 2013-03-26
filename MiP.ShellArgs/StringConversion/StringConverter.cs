using System;
using System.ComponentModel;
using System.Globalization;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.StringConversion
{
    internal class StringConverter : IStringConverter
    {
        private const string CouldNotParseValueToTypeMessage = "Could not parse value '{0}' to type {1}.";

        private readonly IStringParserProvider _parserProvider;

        public StringConverter(IStringParserProvider parserProvider)
        {
            if (parserProvider == null)
                throw new ArgumentNullException("parserProvider");

            _parserProvider = parserProvider;
        }

        public object To(Type targetType, string value)
        {
            try
            {
                // when the target is nullable<T> just get the T
                targetType = targetType.MakeNotNullable();

                IStringParser correctParser = _parserProvider.GetParser(targetType);

                if (correctParser != null)
                    return correctParser.Parse(value);

                // handle enums
                if (targetType.IsEnum)
                    return Enum.Parse(targetType, value, true);

                // try type descriptor
                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
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
    }
}