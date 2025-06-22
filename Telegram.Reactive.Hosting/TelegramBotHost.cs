using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Polling;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Hosting.Services;

namespace Telegram.Reactive.Hosting
{
    public class TelegramBotHost : ITelegramBotHost
    {
        private readonly IHost _host;
        private readonly ITelegramBotClient _client;
        private bool _disposed;

        /// <inheritdoc/>
        public IServiceProvider Services => _host.Services;

        /// <inheritdoc/>
        public TelegramBotOptions Options { get; private set; }

        /// <inheritdoc/>
        public IUpdateRouter UpdateRouter { get; private set; }

        /// <inheritdoc/>
        public ITelegramBotInfo BotInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ILogger<TelegramBotHost> Logger { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomHost"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        internal TelegramBotHost(HostApplicationBuilder hostApplicationBuilder, HostHandlersCollection handlers)
        {
            RegisterHostServices(hostApplicationBuilder.Services, handlers);
            _host = hostApplicationBuilder.Build();

            Options = Services.GetRequiredService<IOptions<TelegramBotOptions>>().Value;
            BotInfo = Services.GetRequiredService<ITelegramBotInfo>();
            UpdateRouter = Services.GetRequiredService<IUpdateRouter>();
            Logger = Services.GetRequiredService<ILogger<TelegramBotHost>>();

            _client = Services.GetRequiredService<ITelegramBotClient>();
            BotInfo.Initialize(_client);
            LogHandlers(handlers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder()
        {
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(null);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder(TelegramBotHostBuilderSettings? settings = null)
        {
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
            return builder;
        }

        /// <summary>
        /// Starts the host.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _host.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Stops the host.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _host.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the host.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            GC.SuppressFinalize(this);
            _host.Dispose();
            _disposed = true;
        }

        private void RegisterHostServices(IServiceCollection hostServices, HostHandlersCollection handlers)
        {
            hostServices.AddSingleton<ITelegramBotHost>(this);
            hostServices.AddSingleton<IReactiveTelegramBot>(this);
            hostServices.AddSingleton<IHandlersCollection>(handlers);
        }

        private void LogHandlers(HostHandlersCollection handlers)
        {
            StringBuilder logBuilder = new StringBuilder("Registered handlers : ");
            if (!handlers.Keys.Any())
                throw new Exception();

            foreach (UpdateType updateType in handlers.Keys)
            {
                HandlerDescriptorList descriptors = handlers[updateType];
                logBuilder.AppendLine("\n\tUpdateType." + updateType + " :");

                foreach (HandlerDescriptor descriptor in descriptors.Reverse())
                {
                    string indexerString = descriptor.Indexer.ToString();
                    logBuilder.AppendLine("* " + indexerString + " " + (descriptor.DisplayString ?? descriptor.HandlerType.Name));
                }
            }

            Logger.LogInformation(logBuilder.ToString());
        }
    }
}
