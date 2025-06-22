using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Providers;

namespace Telegram.Reactive.Handlers
{
    /// <summary>
    /// Container class that holds the context and data for handler execution.
    /// Provides access to the update, client, filters, and other execution context.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    public class AbstractHandlerContainer<TUpdate> : IAbstractHandlerContainer<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// The actual update object of type TUpdate.
        /// </summary>
        private readonly TUpdate _actualUpdate;
        
        /// <summary>
        /// The original update object.
        /// </summary>
        private readonly Update _handlingUpdate;
        
        /// <summary>
        /// The Telegram bot client instance.
        /// </summary>
        private readonly ITelegramBotClient _client;
        
        /// <summary>
        /// Additional data associated with the handler execution.
        /// </summary>
        private readonly Dictionary<string, object> _extraData;
        
        /// <summary>
        /// The list of completed filters for this execution.
        /// </summary>
        private readonly CompletedFiltersList _completedFilters;
        
        /// <summary>
        /// The awaiting provider for managing async operations.
        /// </summary>
        private readonly AwaitingProvider _awaitingProvider;

        /// <summary>
        /// Gets the actual update object of type TUpdate.
        /// </summary>
        public TUpdate ActualUpdate => _actualUpdate;

        /// <inheritdoc/>
        public Update HandlingUpdate => _handlingUpdate;

        /// <inheritdoc/>
        public ITelegramBotClient Client => _client;

        /// <inheritdoc/>
        public Dictionary<string, object> ExtraData => _extraData;

        /// <inheritdoc/>
        public CompletedFiltersList CompletedFilters => _completedFilters;

        /// <inheritdoc cref="IHandlerContainer.AwaitingProvider"/>
        public AwaitingProvider AwaitingProvider => _awaitingProvider;

        /// <inheritdoc/>
        IAwaitingProvider IHandlerContainer.AwaitingProvider => AwaitingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractHandlerContainer{TUpdate}"/> class.
        /// </summary>
        /// <param name="awaitingProvider">The awaiting provider for managing async operations.</param>
        /// <param name="handlerInfo">The handler information containing execution context.</param>
        public AbstractHandlerContainer(AwaitingProvider awaitingProvider, DescribedHandlerInfo handlerInfo)
        {
            _actualUpdate = handlerInfo.HandlingUpdate.GetActualUpdateObject<TUpdate>();
            _handlingUpdate = handlerInfo.HandlingUpdate;
            _client = handlerInfo.Client;
            _extraData = handlerInfo.ExtraData;
            _completedFilters = handlerInfo.CompletedFilters;
            _awaitingProvider = awaitingProvider;
        }
    }
}
