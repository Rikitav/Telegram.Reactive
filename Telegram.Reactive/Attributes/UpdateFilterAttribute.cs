using Telegram.Bot.Types;
using Telegram.Reactive.Attributes.Components;
using Telegram.Reactive.Filters;
using Telegram.Reactive.Filters.Components;

namespace Telegram.Reactive.Attributes
{
    /// <summary>
    /// Abstract base attribute for defining update filters for a specific type of update target.
    /// Provides logic for filter composition, modifier processing, and target extraction.
    /// </summary>
    /// <typeparam name="T">The type of the update target to filter (e.g., Message, Update).</typeparam>
    public abstract class UpdateFilterAttribute<T> : UpdateFilterAttributeBase where T : class
    {
        /// <summary>
        /// Gets the compiled anonymous filter for this attribute.
        /// </summary>
        public override AnonymousTypeFilter AnonymousFilter
        {
            get => AnonymousTypeFilter.Compile(UpdateFilter, GetFilterringTarget);
        }

        /// <summary>
        /// Gets the compiled filter logic for the update target.
        /// </summary>
        public Filter<T> UpdateFilter { get; private set; }

        /// <summary>
        /// Gets or sets the filter modifiers that affect how this filter is combined with others.
        /// </summary>
        public FilterModifier Modifiers { get; set; }

        /// <summary>
        /// Initializes the attribute with one or more filters for the update target.
        /// </summary>
        /// <param name="filters">The filters to compose</param>
        protected UpdateFilterAttribute(params IFilter<T>[] filters)
        {
            UpdateFilter = CompiledFilter<T>.Compile(filters);
        }

        /// <summary>
        /// Initializes the attribute with a precompiled filter for the update target.
        /// </summary>
        /// <param name="updateFilter">The compiled filter</param>
        protected UpdateFilterAttribute(Filter<T> updateFilter)
        {
            UpdateFilter = updateFilter;
        }

        /// <summary>
        /// Processes filter modifiers and combines this filter with the previous one if needed.
        /// </summary>
        /// <param name="previous">The previous filter attribute in the chain</param>
        /// <returns>True if the OrNext modifier is set; otherwise, false.</returns>
        public override sealed bool ProcessModifiers(UpdateFilterAttributeBase previous)
        {
            if (Modifiers.HasFlag(FilterModifier.Not))
                UpdateFilter = UpdateFilter.Not();

            UpdateFilterAttribute<T> previousFilter = (UpdateFilterAttribute<T>)previous;
            if (previousFilter.Modifiers.HasFlag(FilterModifier.OrNext))
            {
                UpdateFilter = previousFilter.UpdateFilter.Or(UpdateFilter);
            }

            return Modifiers.HasFlag(FilterModifier.OrNext);
        }

        /// <summary>
        /// Extracts the filtering target of type <typeparamref name="T"/> from the given update.
        /// </summary>
        /// <param name="update">The Telegram update</param>
        /// <returns>The target object to filter, or null if not applicable</returns>
        public abstract T? GetFilterringTarget(Update update);
    }
}
