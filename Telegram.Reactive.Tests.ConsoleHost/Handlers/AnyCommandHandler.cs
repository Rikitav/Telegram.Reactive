using Telegram.Bot.Types;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.Hosting;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    /// <summary>
    /// Handler for any command with debug environment filtering.
    /// Demonstrates command handling with priority and environment filtering.
    /// </summary>
    [CommandHandler(Priority = int.MinValue), IsDebugEnvironment, DontCollect]
    public class AnyCommandHandler : CommandHandler, IPreBuildingRoutine
    {
        /// <summary>
        /// Pre-building routine that throws NotImplementedException.
        /// </summary>
        /// <param name="hostBuildeer">The host builder instance.</param>
        public static void PreBuildingRoutine(TelegramBotHostBuilder hostBuildeer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the command handler.
        /// Responds with information about the received command.
        /// </summary>
        /// <param name="container">The handler container with message context.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Reply("I received something with command '" + Input.Text + "'", cancellationToken: cancellation);
        }
    }
}
