using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Polling;
using Telegram.Reactive.Providers;

namespace Telegram.Reactive.Hosting.Services
{
    public class HostAwaitingProvider(IOptions<TelegramBotOptions> options, ITelegramBotInfo botInfo, ILogger<HostAwaitingProvider> logger) : AwaitingProvider(options.Value, botInfo)
    {
        public override IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update)
        {
            IEnumerable<DescribedHandlerInfo> handlers = base.GetHandlers(updateRouter, client, update).ToArray();
            logger.LogInformation("Described awaiting handlers : {0}", string.Join(", ", handlers.Select(hndlr => hndlr.HandlerInstance.GetType().Name)));
            return handlers;
        }
    }
}
