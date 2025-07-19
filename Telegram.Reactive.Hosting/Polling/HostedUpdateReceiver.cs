using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Reactive.Hosting.Components;
using Telegram.Reactive.MadiatorCore;
using Telegram.Reactive.Polling;

namespace Telegram.Reactive.Hosting.Polling
{
    public class HostedUpdateReceiver(ITelegramBotHost botHost, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<ReceiverOptions> options, ILogger<HostedUpdateReceiver> logger) : BackgroundService
    {
        private readonly ReceiverOptions ReceiverOptions = options.Value;
        private readonly IUpdateRouter UpdateRouter = updateRouter;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting receiving updates via long-polling");
            ReceiverOptions.AllowedUpdates = botHost.UpdateRouter.HandlersProvider.AllowedTypes.ToArray();
            ReactiveUpdateReceiver updateReceiver = new ReactiveUpdateReceiver(botClient, ReceiverOptions);
            await updateReceiver.ReceiveAsync(UpdateRouter, stoppingToken).ConfigureAwait(false);
        }
    }
}
