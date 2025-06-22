using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    [MessageHandler, ChatType(ChatType.Private), TextEquals("hello")]
    public class PrivateMessageHandlerFirst : MessageHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Client.SendMessage(Input.Chat, "\"" + (Input.Text ?? "<NULL>") + "\" from first private chat! (await 5000)", cancellationToken: cancellation);
            await Task.Delay(5000, cancellation);
            await Client.SendMessage(Input.Chat, "first end", cancellationToken: cancellation);
        }
    }
}