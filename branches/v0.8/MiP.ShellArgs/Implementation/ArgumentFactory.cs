using System;
using System.Globalization;
using System.Linq;

namespace MiP.ShellArgs.Implementation
{
    internal class ArgumentFactory
    {
        private readonly ParserSettings _settings;

        public ArgumentFactory(ParserSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            _settings = settings;
        }

        public Argument Parse(string arg)
        {
            Argument result = new Argument();

            arg = ReadOptionName(arg, result);

            result.Value = arg;

            return result;
        }

        private string ReadOptionName(string arg, Argument result)
        {
            string originalArgument = arg;
            if (!_settings.Prefixes.Any(s => arg.StartsWith(s, StringComparison.OrdinalIgnoreCase) && !arg.Equals(s, StringComparison.OrdinalIgnoreCase)))
                return arg;

            arg = RemoveOptionPrefix(arg);

            int firstAssignment = arg.IndexOfAny(_settings.Assignments);
            if (firstAssignment >= 0)
            {
                // with assignment
                string name = arg.Substring(0, firstAssignment);
                if (string.IsNullOrEmpty(name))
                    return originalArgument;
                
                result.Name = name;

                return arg.Substring(firstAssignment + 1);
            }

            if (_settings.ShortBooleans.Any(c=>arg.EndsWith(c, StringComparison.OrdinalIgnoreCase)))
            {
                // with short boolean
                result.Name = arg.Substring(0, arg.Length - 1);
                return arg.Substring(arg.Length - 1);
            }

            result.Name = arg;
            return string.Empty;
        }

        private string RemoveOptionPrefix(string arg)
        {
            foreach (string optionPrefix in _settings.Prefixes)
            {
                if (arg.StartsWith(optionPrefix, StringComparison.Ordinal))
                {
                    arg = arg.Substring(optionPrefix.Length, arg.Length - optionPrefix.Length);
                    return arg;
                }
            }

            return arg;
        }
    }
}