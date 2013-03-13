using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace MiP.ShellArgs.Implementation
{
    internal class OptionValidator
    {
        private const string OptionNamesNotUniqueMessage = 
            "The following names or aliases are not unique: [{0}].";

        private const string PositionsNotUniqueMessage = 
            "The following options have no unique position: [{0}].";

        private const string MissingPositionMessage = 
            "Option with position {0} was found but position {1} is missing.";

        private const string NonRequiredFollowsOptionalMessage =
            "Positional options must specify all required before optional options, but optional '{0}' was followed by required '{1}'.";

        private const string LastPositionalOptionCollectionMessage = 
            "Only the last positional option may be a collection, but '{0}' and '{1}' are positional collection options.";

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void Validate(ICollection<OptionDefinition> optionDefinitions)
        {
            if (optionDefinitions == null)
                throw new ArgumentNullException("optionDefinitions");

            ValidateNamesAndAliasesAreUnique(optionDefinitions);
            ValidatePositionsAreUnique(optionDefinitions);
            ValidateNoPositionIsMissing(optionDefinitions);
            ValidateNoRequiredFollowsOptional(optionDefinitions);
            ValidateOnlyLastPositionalIsCollection(optionDefinitions);
        }

        private static void ValidateNamesAndAliasesAreUnique(ICollection<OptionDefinition> optionDefinitions)
        {
            IEnumerable<IGrouping<string, string>> aliasesGroups = optionDefinitions.Select(o => o.Name).Concat(optionDefinitions.SelectMany(o => o.Aliases)).GroupBy(o => o);
            string[] nonUniqueAliases = aliasesGroups.Where(g => g.Count() > 1).Select(g => g.Key).ToArray();

            if (nonUniqueAliases.Any())
                throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, OptionNamesNotUniqueMessage, string.Join(", ", nonUniqueAliases)));
        }

        private static void ValidatePositionsAreUnique(IEnumerable<OptionDefinition> optionDefinitions)
        {
            IEnumerable<IGrouping<int, OptionDefinition>> positionGroups = optionDefinitions
                .Where(o => o.Position > 0)
                .GroupBy(o => o.Position);
            
            string[] nonUniquePositions = positionGroups
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Select(gx => gx.Name))
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            if (nonUniquePositions.Any())
                throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, PositionsNotUniqueMessage, string.Join(", ", nonUniquePositions)));
        }

        private static void ValidateNoPositionIsMissing(IEnumerable<OptionDefinition> optionDefinitions)
        {
            IEnumerable<OptionDefinition> positionalOptions = optionDefinitions.Where(o => o.IsPositional).OrderBy(o => o.Position);

            int expected = 1;
            foreach (OptionDefinition positionalOption in positionalOptions)
            {
                if (expected != positionalOption.Position)
                    throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, MissingPositionMessage, positionalOption.Position, expected));

                expected++;
            }
        }

        private static void ValidateNoRequiredFollowsOptional(IEnumerable<OptionDefinition> optionDefinitions)
        {
            IEnumerable<OptionDefinition> positionalOptions = optionDefinitions.Where(o => o.Position > 0).OrderBy(o => o.Position);

            bool lastWasRequired = true;
            OptionDefinition lastOption = null;
            foreach (OptionDefinition positionalOption in positionalOptions)
            {
                if (!lastWasRequired && positionalOption.IsRequired)
                    throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, NonRequiredFollowsOptionalMessage, lastOption.Name, positionalOption.Name));

                lastWasRequired = positionalOption.IsRequired;
                if (!lastWasRequired)
                    lastOption = positionalOption;
            }
        }

        private static void ValidateOnlyLastPositionalIsCollection(IEnumerable<OptionDefinition> optionDefinitions)
        {
            IEnumerable<OptionDefinition> positionalOptions = optionDefinitions.Where(o => o.Position > 0).OrderBy(o => o.Position);

            bool foundICollection = false;
            string lastOptionName = null;
            foreach (OptionDefinition positionalOption in positionalOptions)
            {
                if (foundICollection)
                    throw new ParserInitializationException(string.Format(CultureInfo.InvariantCulture, LastPositionalOptionCollectionMessage, lastOptionName, positionalOption.Name));

                foundICollection = positionalOption.IsCollection;
                lastOptionName = positionalOption.Name;
            }
        }
    }
}