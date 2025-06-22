using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.Personal
{
    [MessageHandler, Mentioned, TextContains("секс")]
    public class SexHandler : MessageHandler
    {
        private const string SexStickerFileid = "CAACAgIAAxkBAAIBNmgmLnaSFNoIwv3VxQiYQ0jVAXWZAAJWLAACCC9gShXOyZwiVBvHNgQ";

        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Client.SendSticker(Input.Chat, InputFile.FromString(SexStickerFileid), Input, cancellationToken: cancellation);
        }
    }
}
