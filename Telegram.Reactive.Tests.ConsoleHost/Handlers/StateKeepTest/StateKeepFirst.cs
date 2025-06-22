using Telegram.Bot.Types;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.Core.Components.StateKeeping;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.StateKeeping;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.StateKeepTest
{
    [CommandHandler(2), CommandAllias("first"), NumericState(SpecialState.NoState), DontCollect]
    public class StateKeepFirst : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            container.CreateNumericState();
            container.ForwardNumericState();
            await Reply("first state moved (1)", cancellationToken: cancellation);
        }
    }
}
