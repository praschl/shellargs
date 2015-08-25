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
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;
        }

        // TODO: Use <TTarget> here in IStringParser to make it type safe for the user
        public void With<TParser>() where TParser : IStringParser, new()
        {
            _settings.RegisterStringParser(new TParser());
        }

        // use <TTarget> here, too!
        public void With<TParser>(TParser parser) where TParser : IStringParser
        {
            if (ReferenceEquals(parser, null))
                throw new ArgumentNullException(nameof(parser));

            _settings.RegisterStringParser(parser);
        }
    }
}