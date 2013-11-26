using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using MiP.ShellArgs.Implementation;
using MiP.ShellArgs.Implementation.Reflection;

namespace MiP.ShellArgs.Fluent
{
    internal class ContainerBuilder<TContainer> : IContainerBuilder<TContainer>
    {
        private const string ExpressionMustBeLikeMessage = 
            "The expression must be either [container => container.Property] or [container => container.Collection.Current()].";

        private const string ContainerDoesNotProvideOptionMessage =
            "The container {0} does not provide the option '{1}'.";

        private readonly List<OptionDefinition> _optionDefinitions = new List<OptionDefinition>();

        internal IEnumerable<OptionDefinition> OptionDefinitions { get { return _optionDefinitions; } }

        private readonly IParserBuilder _parser;
        private readonly PropertyReflector _reflector;

        public ContainerBuilder(TContainer container, IParserBuilder parser, PropertyReflector reflector)
        {
            _parser = parser;
            _reflector = reflector;

            Initialize(container);
        }

        private void Initialize(TContainer container)
        {
            ICollection<OptionDefinition> currentDefinitions = _reflector.CreateOptionDefinitions(typeof (TContainer), container);

            _optionDefinitions.AddRange(currentDefinitions);
        }

        public IContainerCustomizer<TContainer, TArgument> With<TArgument>(string optionName)
        {
            if (optionName == null)
                throw new ArgumentNullException("optionName");

            if (!_optionDefinitions.Any(definition => definition.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase)))
                throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, ContainerDoesNotProvideOptionMessage, typeof(TContainer), optionName));

            return new ContainerCustomizer<TContainer, TArgument>(optionName, _parser, _optionDefinitions, this);
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "propertyExpressions")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "MemberExpressions")]
        public IContainerCustomizer<TContainer, TArgument> With<TArgument>(Expression<Func<TContainer, TArgument>> optionExpression)
        {
            if (optionExpression == null)
                throw new ArgumentNullException("optionExpression");

            MemberInfo member = GetProperty(optionExpression);

            string optionName = _reflector.GetOptionName(member);

            return new ContainerCustomizer<TContainer, TArgument>(optionName, _parser, _optionDefinitions, this);
        }

        private static MemberInfo GetProperty<TArgument>(Expression<Func<TContainer, TArgument>> optionExpression)
        {
            var memberExpression = optionExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                // if it isnt a property access, try if it is a call to the extension method ICollection<T>.CurrentValue()
                var methodCall = optionExpression.Body as MethodCallExpression;
                if (methodCall == null || methodCall.Arguments.Count == 0)
                    throw new NotSupportedException(ExpressionMustBeLikeMessage);

                Expression<Func<ICollection<TArgument>, TArgument>> expectedExpression = x => x.CurrentValue();
                MethodInfo currentValueMethod = ((MethodCallExpression)expectedExpression.Body).Method;

                if (methodCall.Method != currentValueMethod)
                    throw new NotSupportedException(ExpressionMustBeLikeMessage);

                memberExpression = methodCall.Arguments[0] as MemberExpression;
                if (memberExpression == null)
                    throw new NotSupportedException(ExpressionMustBeLikeMessage);
            }

            if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException(ExpressionMustBeLikeMessage);
            if (memberExpression.Member.DeclaringType != typeof(TContainer))
                throw new NotSupportedException(ExpressionMustBeLikeMessage);

            return memberExpression.Member;
        }
    }
}