using System;
using System.Globalization;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs.StringConversion
{
    internal class StringConverter : IStringConverter
    {
        private const string CouldNotParseValueToTypeMessage = "Could not parse value '{0}' to type {1}.";
        private const string CouldNotFindParserMessage = "Could not find a parser which can parse '{0}' to {1}.";

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
                    return correctParser.Parse(targetType, value);

                throw new ParsingException(string.Format(CultureInfo.InvariantCulture, CouldNotFindParserMessage, value, targetType));
            }
            catch (Exception ex)
            {
                throw new ParsingException(string.Format(CultureInfo.InvariantCulture, CouldNotParseValueToTypeMessage, value, targetType.AssemblyQualifiedName), ex);
            }
        }
    }
}