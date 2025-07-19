using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Reactive.Hosting.Web.Polling;

namespace Telegram.Reactive.Hosting.Web
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddTelegramWebhook(this IServiceCollection services)
        {
            services.AddHttpClient<ITelegramBotClient>("tgwebhook").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
            services.AddHostedService<HostedUpdateWebhooker>();
            return services;
        }

        private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
            => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
    }
}
