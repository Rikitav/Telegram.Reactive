using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    [MessageHandler(Priority = int.MinValue), IsDebugEnvironment, TextEquals("any", Modifiers = FilterModifier.Not), DontCollect]
    public class AnyMessageHandler(ILogger<AnyMessageHandler> logger) : MessageHandler
    {
        private readonly ILogger<AnyMessageHandler> _logger = logger;

        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            _logger.LogInformation("Received any message : {0} : {1}", Input.From?.Id ?? -1, Input.Text ?? "<NULL>");
            await Reply("I received a Message", cancellationToken: cancellation);
        }
    }
}
