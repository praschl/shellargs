﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using MiP.ShellArgs.Implementation;

namespace MiP.ShellArgs
{
    /// <summary>
    /// Used to customize the options generated by autowiring.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container.</typeparam>
    public interface IContainerBuilder<TContainer>
    {
        /// <summary>
        /// Customizes the option with the <paramref name="optionName"/>.
        /// </summary>
        /// <typeparam name="TArgument">The type of the option argument.</typeparam>
        /// <param name="optionName">Name of the option to customize.</param>
        /// <returns>A typed customizer to continue with the customization.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "With")]
        IContainerCustomizer<TContainer, TArgument> With<TArgument>(string optionName);

        /// <summary>
        /// Customizes the option identified in the expression.
        /// </summary>
        /// <typeparam name="TArgument">The type of the option argument.</typeparam>
        /// <param name="optionExpression">
        /// Determines the option to customize. Use an expression which returns the value of a property. 
        /// The property is used to determine the name of the option. Only <see cref="MemberExpression" />s are allowed.
        /// <example><c>x=>x.UserName</c></example>
        /// </param>
        /// <returns>A typed customizer to continue with the customization.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "With")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IContainerCustomizer<TContainer, TArgument> With<TArgument>(Expression<Func<TContainer, TArgument>> optionExpression);
    }

    internal interface IContainerBuilder
    {
        IEnumerable<OptionDefinition> OptionDefinitions { get; }

        object Container { get; }
    }
}