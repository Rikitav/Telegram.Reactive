using Telegram.Bot.Types;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.DebugHandlers
{
    [CommandHandler, CommandAllias("args")]
    public class ArgsCommandHandler : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            string replyFormat = string.Format("Received command with arguments : {0}", string.Join(", ", Arguments));
            await Reply(replyFormat, cancellationToken: cancellation);
        }
    }
}
