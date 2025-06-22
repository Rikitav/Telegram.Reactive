using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.Reactive.Core.Configuration
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

        /// <summary>
        /// Initializes the bot info using the specified <see cref="ITelegramBotClient"/>.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task Initialize(ITelegramBotClient botClient);
    }
}
