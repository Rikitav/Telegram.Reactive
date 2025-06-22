using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Configuration;

namespace Telegram.Reactive
{
    /// <summary>
    /// Implementation of <see cref="ITelegramBotInfo"/> that provides bot information.
    /// Contains metadata about the Telegram bot including user details.
    /// </summary>
    public class TelegramBotInfo() : ITelegramBotInfo
    {
        /// <summary>
        /// Gets the user information for the bot.
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// Initializes the bot information by fetching the bot's user details from Telegram.
        /// </summary>
        /// <param name="botClient">The Telegram bot client to use for fetching bot information.</param>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        public async Task Initialize(ITelegramBotClient botClient)
        {
            User = await botClient.GetMe();
        }
    }
}
