using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    [MessageHandler, ChatType(ChatType.Supergroup), TextEquals("hello")]
    public class GroupMessageHandler : MessageHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Client.SendMessage(Input.Chat, "\"" + (Input.Text ?? "<NULL>") + "\" from group chat!", cancellationToken: cancellation);
        }
    }
}
