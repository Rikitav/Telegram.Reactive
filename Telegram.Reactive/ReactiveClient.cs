using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Polling;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Polling;
using Telegram.Reactive.Providers;

namespace Telegram.Reactive
{
    /// <summary>
    /// Main client class for the Telegram.Reactive library.
    /// Extends TelegramBotClient with reactive capabilities for handling updates.
    /// </summary>
    public class ReactiveClient : TelegramBotClient, IReactiveTelegramBot, ICollectingProvider
    {
        /// <summary>
        /// The update router for handling incoming updates.
        /// </summary>
        private IUpdateRouter? updateRouter = null;

        /// <inheritdoc/>
        public TelegramBotOptions Options { get; private set; }

        /// <inheritdoc/>
        public IHandlersCollection Handlers { get; private set; }

        /// <inheritdoc/>
        public ITelegramBotInfo BotInfo { get; private set; }

        /// <inheritdoc/>
        public IUpdateRouter UpdateRouter { get => updateRouter ?? throw new Exception(); }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveClient"/> class with a bot token.
        /// </summary>
        /// <param name="token">The bot token from BotFather.</param>
        /// <param name="httpClient">Optional HTTP client for making requests.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ReactiveClient(string token, HttpClient? httpClient = null, CancellationToken cancellationToken = default)
            : this(new TelegramBotClientOptions(token), httpClient, cancellationToken) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveClient"/> class with bot options.
        /// </summary>
        /// <param name="options">The Telegram bot client options.</param>
        /// <param name="httpClient">Optional HTTP client for making requests.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public ReactiveClient(TelegramBotClientOptions options, HttpClient? httpClient = null, CancellationToken cancellationToken = default) : base(options, httpClient, cancellationToken)
        {
            Options = new TelegramBotOptions();
            Handlers = new HandlersCollection(default);
            BotInfo = new TelegramBotInfo();
            BotInfo.Initialize(this);
        }

        /// <summary>
        /// Starts receiving updates from Telegram.
        /// Initializes the update router and begins polling for updates.
        /// </summary>
        /// <param name="receiverOptions">Optional receiver options for configuring update polling.</param>
        /// <param name="cancellationToken">The cancellation token to stop receiving updates.</param>
        public void StartReceiving(ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default)
        {
            if (Options.GlobalCancellationToken == CancellationToken.None)
                Options.GlobalCancellationToken = cancellationToken;

            HandlersProvider handlerProvider = new HandlersProvider(Handlers.Values, Options, BotInfo);
            AwaitingProvider awaitingProvider = new AwaitingProvider(Options, BotInfo);

            updateRouter = new UpdateRouter(handlerProvider, awaitingProvider, Options);
            StartReceivingInternal(receiverOptions, cancellationToken);
        }

        /// <summary>
        /// Internal method that starts the update receiving process.
        /// Handles the reactive update receiver and error handling.
        /// </summary>
        /// <param name="receiverOptions">Optional receiver options for configuring update polling.</param>
        /// <param name="cancellationToken">The cancellation token to stop receiving updates.</param>
        private async void StartReceivingInternal(ReceiverOptions? receiverOptions, CancellationToken cancellationToken)
        {
            try
            {
                try
                {
                    await new ReactiveUpdateReceiver(this, receiverOptions)
                        .ReceiveAsync(UpdateRouter, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    await UpdateRouter
                        .HandleErrorAsync(this, exception, HandleErrorSource.FatalError, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Cancelled
            }
        }
    }
}
