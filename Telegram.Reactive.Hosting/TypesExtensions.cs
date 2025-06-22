using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Polling;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Hosting.Extensions;
using Telegram.Reactive.Hosting.Services;

namespace Telegram.Reactive.Hosting
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, ConfigureOptionsProxy<TOptions> optionsProxy) where TOptions : class
        {
            optionsProxy.Configure(services, configuration);
            return services;
        }

        public static IServiceCollection AddTelegramBotHostDefaults(this IServiceCollection services)
        {
            services.AddSingleton<IUpdateHandlersPool, HostUpdateHandlersPool>();
            services.AddSingleton<IAwaitingProvider, HostAwaitingProvider>();
            services.AddSingleton<IHandlersProvider, HostHandlersProvider>();
            services.AddSingleton<ITelegramBotInfo, TelegramBotInfo>();
            services.AddSingleton<IUpdateRouter, HostUpdateRouter>();
            services.AddLogging(builder => builder.AddConsole());
            
            return services;
        }

        public static IServiceCollection AddTelegramWebhook(this IServiceCollection services)
        {
            services.AddHttpClient<ITelegramBotClient>("tgwebhook").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
            return services;
        }

        public static IServiceCollection AddTelegramReceiver(this IServiceCollection services)
        {
            services.AddHttpClient<ITelegramBotClient>("tgreceiver").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
            services.AddHostedService<HostedUpdateReceiver>();
            return services;
        }

        private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
            => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
    }

    public static class TelegramBotHostExtensions
    {
        public static TelegramBotHost SetBotCommands(this TelegramBotHost botHost)
        {
            ITelegramBotClient client = botHost.Services.GetRequiredService<ITelegramBotClient>();
            IEnumerable<BotCommand> aliases = botHost.UpdateRouter.HandlersProvider.GetBotCommands();
            client.SetMyCommands(aliases).Wait();
            return botHost;
        }
    }

    public static class TelegramBotHostBuilderExtensions
    {
        /*
        public static ITelegramBotHostBuilder SetAllowedUpdates(this ITelegramBotHostBuilder builder)
        {
            builder.Options.AllowedUpdates = builder.Handlers.Keys.ToArray();
            return builder;
        }
        */
    }
}
