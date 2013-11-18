using System;
using System.Linq;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Implementation
{
    internal class HelpGenerator
    {
        private readonly IStringParserProvider _stringParserProvider;

        public HelpGenerator(IStringParserProvider stringParserProvider)
        {
            if (stringParserProvider == null)
                throw new ArgumentNullException("stringParserProvider");

            _stringParserProvider = stringParserProvider;
            OptionPrefix = '/';
        }

        public char OptionPrefix { get; set; }

        public string GetParameterHelp(params OptionDefinition[] optionInfos)
        {
            string result = string.Join(" ", optionInfos
                .OrderBy(o => o.IsPositional ? o.Position : int.MaxValue)
                .ThenBy(o => !o.IsRequired)
                .ThenBy(o => o.Name)
                .Select(GetParameterHelp));

            return result;
        }

        private string GetParameterHelp(OptionDefinition optionInfo)
        {
            string optionalOpen = string.Empty;
            string optionalClose = string.Empty;
            string positionalOpen = string.Empty;
            string positionalClose = string.Empty;
            string collectionEllipsis = string.Empty;

            if (optionInfo.IsPositional)
            {
                positionalOpen = "[";
                positionalClose = "]";
            }

            if (!optionInfo.IsRequired)
            {
                optionalOpen = "[";
                optionalClose = "]";
            }

            if (optionInfo.IsCollection)
                collectionEllipsis = " [...]";

            return optionalOpen + positionalOpen + OptionPrefix + optionInfo.Name + positionalClose + " " + GetValueDescription(optionInfo) + collectionEllipsis + optionalClose;
        }

        private string GetValueDescription(OptionDefinition optionInfo)
        {
            Type targetType = optionInfo.ValueSetter.ItemType;

            // OptionInfo
            if (!string.IsNullOrEmpty(optionInfo.Description.ValueDescription))
                return optionInfo.Description.ValueDescription;

            // StringToParser
            IStringParser parser = _stringParserProvider.GetParser(targetType);
            if (parser != null && !string.IsNullOrEmpty(parser.ValueDescription))
                return parser.ValueDescription;

            // Enum
            if (targetType.IsEnum && targetType.IsDefined(typeof (FlagsAttribute), false))
                return "(" + string.Join(",", Enum.GetNames(targetType)) + ")";
            if (targetType.IsEnum)
                return "(" + string.Join("|", Enum.GetNames(targetType)) + ")";

            // simple and unknown types
            return GetValueDescription(optionInfo.ValueSetter.ItemType);
        }

        private static string GetValueDescription(Type type)
        {
            if (type == typeof (string))
                return "string";
            if (type == typeof (char))
                return "char";
            if (type == typeof (short))
                return "short";
            if (type == typeof (int))
                return "int";
            if (type == typeof (long))
                return "long";
            if (type == typeof (ushort))
                return "ushort";
            if (type == typeof (uint))
                return "uint";
            if (type == typeof (ulong))
                return "ulong";
            if (type == typeof (bool))
                return "bool";
            if (type == typeof (float))
                return "float";
            if (type == typeof (double))
                return "double";

            if (type == typeof (DateTime))
                return "date";
            if (type == typeof (TimeSpan))
                return "time";

            return type.FullName;
        }
    }
}