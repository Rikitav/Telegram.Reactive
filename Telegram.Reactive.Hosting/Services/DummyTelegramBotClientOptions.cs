using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Telegram.Reactive.Hosting.Services
{
    /// <summary>
    /// Internal dummy class for creating Telegram bot client options.
    /// Provides a simplified way to configure bot client options during host building.
    /// </summary>
    internal class DummyTelegramBotClientOptions
    {
        /// <summary>
        /// Gets or sets the bot token.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the base URL for the bot API.
        /// </summary>
        public string? BaseUrl { get; set; } = null;
        
        /// <summary>
        /// Gets or sets whether to use the test environment.
        /// </summary>
        public bool UseTestEnvironment { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the retry threshold in seconds.
        /// </summary>
        public int RetryThreshold { get; set; } = 60;
        
        /// <summary>
        /// Gets or sets the number of retry attempts.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Creates a real TelegramBotClientOptions instance from the dummy configuration.
        /// </summary>
        /// <returns>An IOptions wrapper containing the configured TelegramBotClientOptions.</returns>
        public IOptions<TelegramBotClientOptions> Realize() => Options.Create(new TelegramBotClientOptions(Token, BaseUrl, UseTestEnvironment)
        {
            RetryCount = RetryCount,
            RetryThreshold = RetryThreshold
        });
    }
}
