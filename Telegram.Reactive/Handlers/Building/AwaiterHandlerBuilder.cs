using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Annotations.StateKeeping;
using Telegram.Reactive.Filters;
using Telegram.Reactive.Filters.Components;
using Telegram.Reactive.Handlers.Building.Components;
using Telegram.Reactive.MadiatorCore;
using Telegram.Reactive.MadiatorCore.Descriptors;
using Telegram.Reactive.StateKeeping;
using Telegram.Reactive.StateKeeping.Components;

namespace Telegram.Reactive.Handlers.Building
{
    /// <summary>
    /// Builder class for creating awaiter handlers that can wait for specific update types.
    /// Provides fluent API for configuring filters, state keepers, and other handler properties.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update to await.</typeparam>
    public class AwaiterHandlerBuilder<TUpdate> : HandlerBuilderBase, IAwaiterHandlerBuilder<TUpdate>, IHandlerBuilderActions<AwaiterHandlerBuilder<TUpdate>> where TUpdate : class
    {
        /// <summary>
        /// The awaiting provider for managing handler registration.
        /// </summary>
        private readonly IAwaitingProvider HandlerProvider;
        
        /// <summary>
        /// The update that triggered the awaiter creation.
        /// </summary>
        private readonly Update HandlingUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AwaiterHandlerBuilder{TUpdate}"/> class.
        /// </summary>
        /// <param name="updateType">The type of update to await.</param>
        /// <param name="handlingUpdate">The update that triggered the awaiter creation.</param>
        /// <param name="handlerProvider">The awaiting provider for managing handler registration.</param>
        /// <exception cref="Exception">Thrown when the update type is not valid for TUpdate.</exception>
        public AwaiterHandlerBuilder(UpdateType updateType, Update handlingUpdate, IAwaitingProvider handlerProvider) : base(typeof(AwaiterHandler), updateType, null)
        {
            if (!updateType.IsValidUpdateObject<TUpdate>())
                throw new Exception();

            HandlerProvider = handlerProvider;
            HandlingUpdate = handlingUpdate;
        }

        /// <summary>
        /// Awaits for an update of the specified type using the default sender ID resolver.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
        /// <returns>The awaited update of type TUpdate.</returns>
        public async Task<TUpdate> Await(CancellationToken cancellationToken = default)
            => await Await(new SenderIdResolver(), cancellationToken);

        /// <summary>
        /// Awaits for an update of the specified type using a custom state key resolver.
        /// </summary>
        /// <param name="keyResolver">The state key resolver to use for filtering updates.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
        /// <returns>The awaited update of type TUpdate.</returns>
        public async Task<TUpdate> Await(IStateKeyResolver<long> keyResolver, CancellationToken cancellationToken = default)
        {
            Filters.Add(new StateKeyFilter<long>(keyResolver, keyResolver.ResolveKey(HandlingUpdate)));
            AwaiterHandler handlerInstance = new AwaiterHandler(UpdateType);
            HandlerDescriptor descriptor = BuildImplicitDescriptor(handlerInstance);
            
            using (HandlerProvider.UseHandler(descriptor))
            {
                handlerInstance.Wait(cancellationToken);
            }

            await Task.CompletedTask;
            return handlerInstance.HandlingUpdate.GetActualUpdateObject<TUpdate>();
        }

        /// <summary>
        /// Sets a custom update validation action.
        /// </summary>
        /// <param name="validateAction">The validation action to apply to updates.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> SetUpdateValidating(UpdateValidateAction validateAction)
        {
            ValidateFilter = new UpdateValidateFilter(validateAction);
            return this;
        }

        /// <summary>
        /// Sets the concurrency limit for the handler.
        /// </summary>
        /// <param name="concurrency">The maximum number of concurrent executions allowed.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> SetConcurreny(int concurrency)
        {
            Indexer = Indexer.UpdateConcurrency(concurrency);
            return this;
        }

        /// <summary>
        /// Sets the priority for the handler.
        /// </summary>
        /// <param name="priority">The priority value for the handler.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> SetPriority(int priority)
        {
            Indexer = Indexer.UpdatePriority(priority);
            return this;
        }

        /// <summary>
        /// Sets both concurrency and priority for the handler.
        /// </summary>
        /// <param name="concurrency">The maximum number of concurrent executions allowed.</param>
        /// <param name="priority">The priority value for the handler.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> SetIndexer(int concurrency, int priority)
        {
            Indexer = new DescriptorIndexer(concurrency, priority);
            return this;
        }

        /// <summary>
        /// Adds a filter to the handler.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> AddFilter(IFilter<Update> filter)
        {
            Filters.Add(filter);
            return this;
        }

        /// <summary>
        /// Adds multiple filters to the handler.
        /// </summary>
        /// <param name="filters">The filters to add.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> AddFilters(params IFilter<Update>[] filters)
        {
            Filters.AddRange(filters);
            return this;
        }

        /// <summary>
        /// Sets a state keeper with a specific state value.
        /// </summary>
        /// <typeparam name="TKey">The type of the state key.</typeparam>
        /// <typeparam name="TState">The type of the state value.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="myState">The state value to keep.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> SetStateKeeper<TKey, TState, TKeeper>(TState myState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            StateKeeper = new StateKeepFilter<TKey, TState, TKeeper>(myState, keyResolver);
            return this;
        }

        /// <summary>
        /// Sets a state keeper with a special state.
        /// </summary>
        /// <typeparam name="TKey">The type of the state key.</typeparam>
        /// <typeparam name="TState">The type of the state value.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="specialState">The special state to keep.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> SetStateKeeper<TKey, TState, TKeeper>(SpecialState specialState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            StateKeeper = new StateKeepFilter<TKey, TState, TKeeper>(specialState, keyResolver);
            return this;
        }

        /// <summary>
        /// Adds a targeted filter that operates on a specific target type extracted from the update.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to extract the filter target from the update.</param>
        /// <param name="filter">The filter to apply to the target.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> AddTargetedFilter<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter) where TFilterTarget : class
        {
            AnonymousTypeFilter anonymousTypeFilter = AnonymousTypeFilter.Compile(filter, getFilterringTarget);
            Filters.Add(anonymousTypeFilter);
            return this;
        }

        /// <summary>
        /// Adds multiple targeted filters that operate on a specific target type extracted from the update.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to extract the filter target from the update.</param>
        /// <param name="filters">The filters to apply to the target.</param>
        /// <returns>This builder instance for method chaining.</returns>
        public AwaiterHandlerBuilder<TUpdate> AddTargetedFilters<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters) where TFilterTarget : class
        {
            AnonymousCompiledFilter compiledPollingFilter = AnonymousCompiledFilter.Compile(filters, getFilterringTarget);
            Filters.Add(compiledPollingFilter);
            return this;
        }
    }
}
