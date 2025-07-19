using Microsoft.Extensions.Hosting;

namespace Telegram.Reactive.Hosting.Components
{
    /// <summary>
    /// Interface for Telegram bot hosts.
    /// Combines host application capabilities with reactive Telegram bot functionality.
    /// </summary>
    public interface ITelegramBotHost : IHost, IReactiveTelegramBot
    {

    }
}
