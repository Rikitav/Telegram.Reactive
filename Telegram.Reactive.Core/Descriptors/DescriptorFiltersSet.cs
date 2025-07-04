﻿using Telegram.Bot.Types;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Components.Filters;

namespace Telegram.Reactive.Core.Descriptors
{
    /// <summary>
    /// Represents a set of filters for a handler descriptor, including update and state keeper validators.
    /// </summary>
    public sealed class DescriptorFiltersSet
    {
        /// <summary>
        /// Validator for the update object.
        /// </summary>
        public IFilter<Update>? UpdateValidator { get; set; }

        /// <summary>
        /// Validator for the state keeper.
        /// </summary>
        public IFilter<Update>? StateKeeperValidator { get; set; }

        /// <summary>
        /// Array of update filters.
        /// </summary>
        public IFilter<Update>[]? UpdateFilters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorFiltersSet"/> class.
        /// </summary>
        /// <param name="updateValidator">Validator for the update object.</param>
        /// <param name="stateKeeperValidator">Validator for the state keeper.</param>
        /// <param name="updateFilters">Array of update filters.</param>
        public DescriptorFiltersSet(IFilter<Update>? updateValidator, IFilter<Update>? stateKeeperValidator, IFilter<Update>[]? updateFilters)
        {
            UpdateValidator = updateValidator;
            StateKeeperValidator = stateKeeperValidator;
            UpdateFilters = updateFilters;
        }

        /// <summary>
        /// Validates the filter context using all filters in the set.
        /// </summary>
        /// <param name="filterContext">The filter execution context.</param>
        /// <returns>True if all filters pass; otherwise, false.</returns>
        public bool Validate(FilterExecutionContext<Update> filterContext)
        {
            if (UpdateValidator != null)
            {
                if (!UpdateValidator.CanPass(filterContext))
                    return false;

                filterContext.CompletedFilters.Add(UpdateValidator);
            }

            if (StateKeeperValidator != null)
            {
                if (!StateKeeperValidator.CanPass(filterContext))
                    return false;

                filterContext.CompletedFilters.Add(StateKeeperValidator);
            }

            if (UpdateFilters != null)
            {
                if (!UpdateFilters.All(filter => filter.CanPass(filterContext)))
                    return false;

                filterContext.CompletedFilters.AddRange(UpdateFilters);
            }

            return true;
        }
    }
}
