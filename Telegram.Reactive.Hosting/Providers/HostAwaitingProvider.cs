using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Configuration;
using Telegram.Reactive.MadiatorCore;
using Telegram.Reactive.MadiatorCore.Descriptors;
using Telegram.Reactive.Providers;

namespace Telegram.Reactive.Hosting.Providers
{
    public class HostAwaitingProvider(IOptions<TelegramBotOptions> options, ITelegramBotInfo botInfo, ILogger<HostAwaitingProvider> logger) : AwaitingProvider(options.Value, botInfo)
    {
        public override IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            IEnumerable<DescribedHandlerInfo> handlers = base.GetHandlers(updateRouter, client, update, cancellationToken).ToArray();
            logger.LogInformation("Described awaiting handlers : {handlers}", string.Join(", ", handlers.Select(hndlr => hndlr.HandlerInstance.GetType().Name)));
            return handlers;
        }
    }
}
