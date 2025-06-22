using Telegram.Bot.Types;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.StateKeeping;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.StateKeepTest
{
    [CommandHandler(2), CommandAllias("second"), NumericState(2), DontCollect]
    public class StateKeeperSecond : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            container.ForwardNumericState();
            await Reply("second state moved (2)", cancellationToken: cancellation);
        }
    }
}
