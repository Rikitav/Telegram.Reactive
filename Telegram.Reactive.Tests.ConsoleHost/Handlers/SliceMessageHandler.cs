using Telegram.Bot.Types;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    [CommandHandler, CommandAllias("slice")]
    public class SliceMessageHandler : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            if (string.IsNullOrEmpty(ArgumentsString))
            {
                await Reply("Send arguments", cancellationToken: cancellation);
                return;
            }

            foreach (string slice in ArgumentsString.SliceBy(10))
                await Responce(slice, cancellationToken: cancellation);
        }
    }
}
