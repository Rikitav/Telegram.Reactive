﻿namespace Telegram.Reactive.Core.Components.Filters
{
    /// <summary>
    /// Represents a filter that composes multiple filters and passes only if all of them pass.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class CompiledFilter<T> : Filter<T> where T : class
    {
        private readonly IFilter<T>[] Filters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledFilter{T}"/> class.
        /// </summary>
        /// <param name="filters">The filters to compose.</param>
        private CompiledFilter(IFilter<T>[] filters)
        {
            Filters = filters;
        }

        /// <summary>
        /// Compiles multiple filters into a <see cref="CompiledFilter{T}"/>.
        /// </summary>
        /// <param name="filters">The filters to compose.</param>
        /// <returns>A new <see cref="CompiledFilter{T}"/> instance.</returns>
        public static CompiledFilter<T> Compile(params IFilter<T>[] filters)
        {
            return new CompiledFilter<T>(filters);
        }

        /// <summary>
        /// Determines whether all composed filters pass for the given context.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if all filters pass; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<T> context)
        {
            return Filters.All(filter => filter.CanPass(context));
        }
    }
}
