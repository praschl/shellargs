using System;

namespace MiP.ShellArgs.StringConversion
{
    internal interface IStringParserProvider
    {
        IStringParser GetParser(Type targetType);

        void RegisterParser(IStringParser parser);
    }
}