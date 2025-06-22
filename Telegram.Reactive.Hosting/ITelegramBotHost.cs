using Microsoft.Extensions.Hosting;

namespace Telegram.Reactive.Hosting
{
    /// <summary>
    /// Interface for Telegram bot hosts that integrate with Microsoft.Extensions.Hosting.
    /// Combines host application capabilities with reactive Telegram bot functionality.
    /// </summary>
    public interface ITelegramBotHost : IHost, IReactiveTelegramBot
    {
    }
}
