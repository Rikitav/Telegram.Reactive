using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    [MessageHandler, ChatType(ChatType.Private), TextEquals("hello")]
    public class PrivateMessageHandlerThird : MessageHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Client.SendMessage(Input.Chat, "\"" + (Input.Text ?? "<NULL>") + "\" from third private chat! (await 2500)", cancellationToken: cancellation);
            await Task.Delay(2500, cancellation);
            await Client.SendMessage(Input.Chat, "third end", cancellationToken: cancellation);
        }
    }
}