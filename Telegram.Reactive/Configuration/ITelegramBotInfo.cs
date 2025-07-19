using Telegram.Bot.Types;

namespace Telegram.Reactive.Configuration
{
    /// <summary>
    /// Interface for providing bot information and metadata.
    /// Contains information about the bot user and provides initialization capabilities.
    /// </summary>
    public interface ITelegramBotInfo
    {
        /// <summary>
        /// Gets the <see cref="User"/> representing the bot.
        /// </summary>
        public User User { get; }
    }
}
