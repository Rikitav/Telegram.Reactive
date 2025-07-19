using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Annotations.StateKeeping;
using Telegram.Reactive.Filters.Components;
using Telegram.Reactive.Handlers.Building.Components;
using Telegram.Reactive.MadiatorCore;
using Telegram.Reactive.MadiatorCore.Descriptors;
using Telegram.Reactive.StateKeeping.Components;

namespace Telegram.Reactive.Handlers.Building
{
    /// <summary>
    /// Delegate for handler execution actions that take a container and cancellation token.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    /// <param name="container">The handler container with execution context.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>A task representing the asynchronous execution.</returns>
    public delegate Task AbstractHandlerAction<TUpdate>(IAbstractHandlerContainer<TUpdate> container, CancellationToken cancellation) where TUpdate : class;

    /// <summary>
    /// Builder class for creating regular handlers that can process updates.
    /// Provides fluent API for configuring filters, state keepers, and other handler properties.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update to handle.</typeparam>
    public class HandlerBuilder<TUpdate> : HandlerBuilderBase, IRegularHandlerBuilder<TUpdate>, IHandlerBuilderActions<HandlerBuilder<TUpdate>> where TUpdate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerBuilder{TUpdate}"/> class.
        /// </summary>
        /// <param name="updateType">The type of update this handler will process.</param>
        /// <param name="handlerCollection">The collection to register the built handler with.</param>
        /// <exception cref="ArgumentException">Thrown when the update type is not valid for TUpdate.</exception>
        public HandlerBuilder(UpdateType updateType, IHandlersCollection handlerCollection) : base(typeof(BuildedAbstractHandler<TUpdate>), updateType, handlerCollection)
        {
            if (!updateType.IsValidUpdateObject<TUpdate>())
                throw new ArgumentException("\"UpdateType." + updateType + "\" is not valid type for \"" + nameof(TUpdate) + "\" update object", nameof(updateType));
        }

        /// <summary>
        /// Builds an abstract handler with the specified execution action.
        /// </summary>
        /// <param name="executeHandler">The delegate action to execute when the handler is invoked.</param>
        /// <exception cref="ArgumentNullException">Thrown when executeHandler is null.</exception>
        public void Build(AbstractHandlerAction<TUpdate> executeHandler)
        {
            if (executeHandler == null)
                throw new ArgumentNullException(nameof(executeHandler));

            BuildedAbstractHandler<TUpdate> instance = new BuildedAbstractHandler<TUpdate>(UpdateType, executeHandler);
            BuildImplicitDescriptor(instance);
        }

        /// <summary>
        /// Sets a custom update validation action.
        /// </summary>
        /// <param name="validateAction">The validation action to apply to updates.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> SetUpdateValidating(UpdateValidateAction validateAction)
        {
            ValidateFilter = new UpdateValidateFilter(validateAction);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Sets the concurrency limit for the handler.
        /// </summary>
        /// <param name="concurrency">The maximum number of concurrent executions allowed.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> SetConcurreny(int concurrency)
        {
            Indexer = Indexer.UpdateConcurrency(concurrency);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Sets the priority for the handler.
        /// </summary>
        /// <param name="priority">The priority value for the handler.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> SetPriority(int priority)
        {
            Indexer = Indexer.UpdatePriority(priority);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Sets both concurrency and priority for the handler.
        /// </summary>
        /// <param name="concurrency">The maximum number of concurrent executions allowed.</param>
        /// <param name="priority">The priority value for the handler.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> SetIndexer(int concurrency, int priority)
        {
            Indexer = new DescriptorIndexer(concurrency, priority);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Adds a filter to the handler.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> AddFilter(IFilter<Update> filter)
        {
            Filters.Add(filter);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Adds multiple filters to the handler.
        /// </summary>
        /// <param name="filters">The filters to add.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> AddFilters(params IFilter<Update>[] filters)
        {
            Filters.AddRange(filters);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Sets a state keeper with a specific state value.
        /// </summary>
        /// <typeparam name="TKey">The type of the state key.</typeparam>
        /// <typeparam name="TState">The type of the state value.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="myState">The state value to keep.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> SetStateKeeper<TKey, TState, TKeeper>(TState myState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            StateKeeper = new StateKeepFilter<TKey, TState, TKeeper>(myState, keyResolver);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Sets a state keeper with a special state.
        /// </summary>
        /// <typeparam name="TKey">The type of the state key.</typeparam>
        /// <typeparam name="TState">The type of the state value.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="specialState">The special state to keep.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> SetStateKeeper<TKey, TState, TKeeper>(SpecialState specialState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            StateKeeper = new StateKeepFilter<TKey, TState, TKeeper>(specialState, keyResolver);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Adds a targeted filter that operates on a specific target type extracted from the update.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to extract the filter target from the update.</param>
        /// <param name="filter">The filter to apply to the target.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> AddTargetedFilter<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter) where TFilterTarget : class
        {
            AnonymousTypeFilter anonymousTypeFilter = AnonymousTypeFilter.Compile(filter, getFilterringTarget);
            Filters.Add(anonymousTypeFilter);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Adds multiple targeted filters that operate on a specific target type extracted from the update.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to extract the filter target from the update.</param>
        /// <param name="filters">The filters to apply to the target.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public HandlerBuilder<TUpdate> AddTargetedFilters<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters) where TFilterTarget : class
        {
            AnonymousCompiledFilter compiledPollingFilter = AnonymousCompiledFilter.Compile(filters, getFilterringTarget);
            Filters.Add(compiledPollingFilter);
            return this;
        }
    }
}
