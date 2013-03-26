using System;
using System.Collections.Generic;
using System.Linq;

namespace MiP.ShellArgs.StringConversion
{
    internal class StringParserProvider : IStringParserProvider
    {
        private readonly Stack<IStringParser> _parsers = new Stack<IStringParser>();

        public StringParserProvider()
        {
            RegisterParser(new StringToBoolParser());
            RegisterParser(new StringToDateTimeParser());
            RegisterParser(new StringToTimeSpanParser());
        }

        // TODO: test
        public IStringParser GetParser(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            
            return _parsers.FirstOrDefault(p => p.CanParseTo(targetType));
        }

        // TODO: test
        public void RegisterParser(IStringParser parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            _parsers.Push(parser);
        }
    }
}