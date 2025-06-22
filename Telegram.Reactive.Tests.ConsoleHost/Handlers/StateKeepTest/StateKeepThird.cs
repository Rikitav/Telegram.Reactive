using Telegram.Bot.Types;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.StateKeeping;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.StateKeepTest
{
    [CommandHandler(2), CommandAllias("third"), NumericState(3), DontCollect]
    public class StateKeepThird : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            container.ForwardNumericState();
            await Reply("third state moved (3)", cancellationToken: cancellation);
        }
    }
}
