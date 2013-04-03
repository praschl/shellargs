using System;
using System.Collections.Generic;
using System.Linq;

namespace MiP.ShellArgs.StringConversion
{
    internal class StringParserProvider : IStringParserProvider
    {
        private readonly Stack<IStringParser> _parsers = new Stack<IStringParser>();

        internal ParserSettings ParserSettings { get; private set; }

        public StringParserProvider(ParserSettings parserSettings)
        {
            ParserSettings = parserSettings;
            RegisterParser(new StringToObjectParser());
            // TODO: register KeyValuePair parser
            //RegisterParser(new StringToKeyValuePairParser(this, parserSettings));
            RegisterParser(new StringToEnumParser());
            RegisterParser(new StringToBoolParser());
        }

        public IStringParser GetParser(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            return _parsers.FirstOrDefault(p => p.CanParseTo(targetType));
        }

        public void RegisterParser(IStringParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            _parsers.Push(parser);
        }
    }
}