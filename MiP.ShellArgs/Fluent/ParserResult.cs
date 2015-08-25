using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MiP.ShellArgs.Fluent
{
    internal class ParserResult : IParserResult
    {
        private const string UnknownArgumentContainerTypeMessage =
            "Type {0} is not a known argument container type, add it with RegisterContainer<T>(), RegisterContainer<T>(T instance) or Parse<T>().";

        private readonly IDictionary<Type, object> _instances;

        public ParserResult(IDictionary<Type, object> instances)
        {
            if (instances == null)
                throw new ArgumentNullException(nameof(instances));

            _instances = instances;
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RegisterContainer", Justification = "Design time message")]
        public TContainer Result<TContainer>()
        {
            if (!_instances.ContainsKey(typeof (TContainer)))
                throw new KeyNotFoundException(string.Format(CultureInfo.InvariantCulture, UnknownArgumentContainerTypeMessage, typeof (TContainer)));

            return (TContainer)_instances[typeof (TContainer)];
        }

        public IParserResult ResultTo<TContainer>(out TContainer result)
        {
            result = Result<TContainer>();
            return this;
        }
    }
}