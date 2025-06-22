using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.Handlers.Building;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers
{
    [CommandHandler, CommandAllias("await")]
    public class AwaitingCallbackQueryHandler : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            InlineKeyboardButton[] buttons = [new InlineKeyboardButton("Left", "callback_left"), new InlineKeyboardButton("Right", "callback_right")];
            await Reply("Choose your side", replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken: cancellation);

            CallbackQuery callbackQuery = await Container.AwaitCallbackQuery().Await(cancellation);
            await Reply(callbackQuery.Data ?? "<null>", cancellationToken: cancellation);
        }
    }
}
