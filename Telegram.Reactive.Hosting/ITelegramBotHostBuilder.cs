using Microsoft.Extensions.Hosting;
using Telegram.Reactive.Core.Providers;

namespace Telegram.Reactive.Hosting
{
    /// <summary>
    /// Interface for building Telegram bot hosts with dependency injection support.
    /// Combines host application building capabilities with handler collection functionality.
    /// </summary>
    public interface ITelegramBotHostBuilder : IHostApplicationBuilder, ICollectingProvider
    {

    }
}
