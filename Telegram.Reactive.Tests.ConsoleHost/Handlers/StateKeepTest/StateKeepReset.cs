using Telegram.Bot.Types;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.Core.Components.StateKeeping;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.StateKeeping;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.StateKeepTest
{
    [CommandHandler(2), CommandAllias("resetstate"), NumericState(SpecialState.AnyState), DontCollect]
    public class StateKeepReset : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            container.SetNumericState(null);
            await Reply("state reseted (0)", cancellationToken: cancellation);
        }
    }
}
