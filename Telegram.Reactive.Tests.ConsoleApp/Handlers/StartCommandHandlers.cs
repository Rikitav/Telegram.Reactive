using Telegram.Bot.Types;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleApp.Handlers
{
    /// <summary>
    /// Handler for the /start command.
    /// Responds to users who start the bot with a simple message.
    /// </summary>
    [CommandHandler, CommandAllias("start")]
    public class StartCommandHandlers : CommandHandler
    {
        /// <summary>
        /// Executes the start command handler.
        /// Sends a "started" message to the user.
        /// </summary>
        /// <param name="container">The handler container with message context.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Reply("started", cancellationToken: cancellation);
        }
    }
}
