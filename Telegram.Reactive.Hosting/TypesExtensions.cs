using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Configuration;
using Telegram.Reactive.Hosting.Components;
using Telegram.Reactive.Hosting.Configuration;
using Telegram.Reactive.Hosting.Polling;
using Telegram.Reactive.Hosting.Providers;
using Telegram.Reactive.Hosting.Services;
using Telegram.Reactive.MadiatorCore;

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
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<IUpdateHandlersPool, HostUpdateHandlersPool>();
            services.AddSingleton<IAwaitingProvider, HostAwaitingProvider>();
            services.AddSingleton<IHandlersProvider, HostHandlersProvider>();
            services.AddSingleton<IUpdateRouter, HostUpdateRouter>();
            services.AddSingleton<ITelegramBotInfo, TelegramBotInfo>(services => new TelegramBotInfo(services.GetRequiredService<ITelegramBotClient>().GetMe().Result));
            
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
        public static ITelegramBotHost SetBotCommands(this ITelegramBotHost botHost)
        {
            ITelegramBotClient client = botHost.Services.GetRequiredService<ITelegramBotClient>();
            IEnumerable<BotCommand> aliases = botHost.UpdateRouter.HandlersProvider.GetBotCommands();
            client.SetMyCommands(aliases).Wait();
            return botHost;
        }
    }
}
