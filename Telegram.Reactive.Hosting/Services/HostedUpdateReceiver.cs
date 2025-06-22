using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Reactive.Core.Polling;
using Telegram.Reactive.Polling;

namespace Telegram.Reactive.Hosting.Services
{
    public class HostedUpdateReceiver(ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<ReceiverOptions> options, ILogger<HostedUpdateReceiver> logger) : BackgroundService
    {
        private readonly ITelegramBotClient Client = botClient;
        private readonly ReceiverOptions ReceiverOptions = options.Value;
        private readonly IUpdateRouter UpdateRouter = updateRouter;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting receiving updates via long-polling");
            ReactiveUpdateReceiver updateReceiver = new ReactiveUpdateReceiver(Client, ReceiverOptions);
            await updateReceiver.ReceiveAsync(UpdateRouter, stoppingToken).ConfigureAwait(false);
        }
    }
}
