using System;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Fluent
{
    internal class ParserConfigurator<TTarget> : IParserConfigurator<TTarget>
    {
        private readonly ParserSettings _settings;

        public ParserConfigurator(ParserSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings = settings;
        }

        public ParserSettings With<TParser>() where TParser : IStringParser, new()
        {
            _settings.RegisterStringParser<TParser, TTarget>(new TParser());

            return _settings;
        }

        public ParserSettings With<TParser>(TParser parser) where TParser : IStringParser
        {
            if (ReferenceEquals(parser, null))
                throw new ArgumentNullException("parser");

            _settings.RegisterStringParser<TParser, TTarget>(parser);

            return _settings;
        }
    }
}