using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Polling;

namespace Telegram.Reactive
{
    /// <summary>
    /// Interface for reactive Telegram bot implementations.
    /// Defines the core properties and capabilities of a reactive bot.
    /// </summary>
    public interface IReactiveTelegramBot
    {
        /// <summary>
        /// Gets the configuration options for the Telegram bot.
        /// </summary>
        public TelegramBotOptions Options { get; }

        /// <summary>
        /// Gets the update router for handling incoming updates.
        /// </summary>
        public IUpdateRouter UpdateRouter { get; }

        /// <summary>
        /// Gets the bot information and metadata.
        /// </summary>
        public ITelegramBotInfo BotInfo { get; }
    }
}
