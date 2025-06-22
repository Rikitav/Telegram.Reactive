using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Hosting;
using Telegram.Reactive.Tests.ConsoleHost.Handlers.StateKeepTest;

namespace Telegram.Reactive.Tests.ConsoleHost
{
    /// <summary>
    /// Main program class for the Telegram.Reactive.Tests.ConsoleHost project.
    /// Demonstrates advanced usage of the TelegramBotHost with state keeping and custom handlers.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point for the console host test application.
        /// Creates a Telegram bot host with custom configuration and handlers.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            TelegramBotHostBuilder builder = TelegramBotHost.CreateBuilder(new TelegramBotHostBuilderSettings()
            {
                Args = args,
                ApplicationName = "RayaPrime-Host",
                DescendConflictingPriority = false,
                ExceptIntersectingCommandAliases = true
            });

            builder.Handlers.CollectHandlersDomainWide();
            builder.Handlers
                .AddHandler<StateKeeperSecond>()
                .AddHandler<StateKeepFirst>()
                .AddHandler<StateKeepReset>()
                .AddHandler<StateKeepThird>();

            builder.Handlers.CreateMessage().ChatType(ChatType.Private).TextContains("fuck you", StringComparison.InvariantCultureIgnoreCase).Build(async (context, cancell) =>
            {
                await context.Client.SendMessage(context.ActualUpdate.Chat, "fuck you too", replyParameters: context.ActualUpdate.Id, cancellationToken: cancell);
            });
            
            TelegramBotHost telegramBot = builder.Build();
            telegramBot.SetBotCommands();
            telegramBot.Run();
        }

        /*
        [MessageHandler, TextEquals("method")]
        public async Task HandlerInMethod(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await container.Client.SendMessage(container.ActualUpdate.Chat, "from handler in method", cancellationToken: cancellation);
        }
        */
    }
}
