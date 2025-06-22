using Telegram.Bot.Types;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.Core.Components.StateKeeping;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.StateKeeping;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.StateKeepTest
{
    [MessageHandler(2), NumericState(SpecialState.AnyState), DontCollect]
    public class StateKeepAny : MessageHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Reply("any state", cancellationToken: cancellation);
        }
    }
}
