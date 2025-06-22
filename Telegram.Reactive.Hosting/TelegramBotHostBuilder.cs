using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Hosting.Extensions;
using Telegram.Reactive.Hosting.Services;

namespace Telegram.Reactive.Hosting
{
    public class TelegramBotHostBuilder : ITelegramBotHostBuilder
    {
        private readonly HostApplicationBuilder HostApplicationBuilder;
        private readonly TelegramBotHostBuilderSettings Settings;

        /// <inheritdoc cref="ICollectingProvider.Handlers"/>
        public HostHandlersCollection Handlers { get; private set; }

        /// <inheritdoc/>
        public IServiceCollection Services => HostApplicationBuilder.Services;

        /// <inheritdoc/>
        public IDictionary<object, object> Properties => ((IHostApplicationBuilder)HostApplicationBuilder).Properties;

        /// <inheritdoc/>
        public IConfigurationManager Configuration => HostApplicationBuilder.Configuration;

        /// <inheritdoc/>
        public IHostEnvironment Environment => HostApplicationBuilder.Environment;

        /// <inheritdoc/>
        public ILoggingBuilder Logging => HostApplicationBuilder.Logging;

        /// <inheritdoc/>
        public IMetricsBuilder Metrics => HostApplicationBuilder.Metrics;

        IHandlersCollection ICollectingProvider.Handlers => Handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        internal TelegramBotHostBuilder(TelegramBotHostBuilderSettings? settings)
        {
            // Inner builder
            Settings = settings ?? new TelegramBotHostBuilderSettings();
            HostApplicationBuilder = new HostApplicationBuilder(Settings.ToApplicationBuilderSettings());
            Handlers = new HostHandlersCollection(Services, Settings);

            Services.Configure<TelegramBotOptions>(Configuration.GetSection(nameof(TelegramBotOptions)));
            Services.Configure<ReceiverOptions>(Configuration.GetSection(nameof(ReceiverOptions)));
            Services.Configure(Configuration.GetSection(nameof(TelegramBotClientOptions)), new TelegramBotClientOptionsProxy());
        }

        /// <summary>
        /// Builds the host.
        /// </summary>
        /// <returns>A custom host instance.</returns>
        public TelegramBotHost Build()
        {
            foreach (var preBuildRoutine in Handlers.PreBuilderRoutines)
            {
                try
                {
                    preBuildRoutine.Invoke(this);
                }
                catch (NotImplementedException)
                {
                    _ = 0xBAD + 0xC0DE;
                }
            }

            return new TelegramBotHost(HostApplicationBuilder, Handlers);
        }

        public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
        {
            HostApplicationBuilder.ConfigureContainer(factory, configure);
        }
    }
}
