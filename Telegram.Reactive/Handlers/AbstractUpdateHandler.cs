using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Providers;

namespace Telegram.Reactive.Handlers
{
    /// <summary>
    /// Abstract handler for Telegram updates of type <typeparamref name="TUpdate"/>.
    /// </summary>
    public abstract class AbstractUpdateHandler<TUpdate> : UpdateHandlerBase, IHandlerContainerFactory where TUpdate : class
    {
        /// <summary>
        /// Handler container for the current update.
        /// </summary>
        protected AbstractHandlerContainer<TUpdate> Container { get; private set; } = default!;

        /// <summary>
        /// Telegram Bot client associated with the current container.
        /// </summary>
        protected ITelegramBotClient Client => Container.Client;

        /// <summary>
        /// Incoming update of type <typeparamref name="TUpdate"/>.
        /// </summary>
        protected TUpdate Input => Container.ActualUpdate;

        /// <summary>
        /// The Telegram update being handled.
        /// </summary>
        protected Update HandlingUpdate => Container.HandlingUpdate;

        /// <summary>
        /// Additional data associated with the handler execution.
        /// </summary>
        protected Dictionary<string, object> ExtraData => Container.ExtraData;

        /// <summary>
        /// List of successfully passed filters.
        /// </summary>
        protected CompletedFiltersList CompletedFilters => Container.CompletedFilters;

        /// <summary>
        /// Provider for awaiting asynchronous operations.
        /// </summary>
        protected AwaitingProvider AwaitingProvider => Container.AwaitingProvider;

        /// <summary>
        /// Initializes a new instance and checks that the update type matches <typeparamref name="TUpdate"/>.
        /// </summary>
        /// <param name="handlingUpdateType">The type of update to handle.</param>
        protected AbstractUpdateHandler(UpdateType handlingUpdateType) : base(handlingUpdateType)
        {
            if (!HandlingUpdateType.IsValidUpdateObject<TUpdate>())
                throw new Exception();
        }

        /// <summary>
        /// Creates a handler container for the specified awaiting provider and handler info.
        /// </summary>
        /// <param name="awaitingProvider">The awaiting provider.</param>
        /// <param name="handlerInfo">The handler descriptor info.</param>
        /// <returns>The created handler container.</returns>
        public virtual IHandlerContainer CreateContainer(IAwaitingProvider awaitingProvider, DescribedHandlerInfo handlerInfo)
        {
            if (awaitingProvider is not AwaitingProvider _awaitingProvider)
                throw new Exception();
            
            return new AbstractHandlerContainer<TUpdate>(_awaitingProvider, handlerInfo);
        }

        /// <summary>
        /// Executes the handler logic using the specified container.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override sealed async Task Execute(IHandlerContainer container, CancellationToken cancellationToken)
        {
            Container = (AbstractHandlerContainer<TUpdate>)container;
            await Execute(Container, cancellationToken);
        }

        /// <summary>
        /// Abstract method to execute the update handling logic.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public abstract Task Execute(AbstractHandlerContainer<TUpdate> container, CancellationToken cancellation);
    }
}
