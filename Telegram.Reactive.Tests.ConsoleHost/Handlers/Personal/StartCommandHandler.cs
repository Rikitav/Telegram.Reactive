using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.Personal
{
    [CommandHandler, CommandAllias("start", "hello"), ChatType(ChatType.Private), IsDebugEnvironment]
    public class StartCommandHandler : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            if (Input.From is not { Id: > 0 })
                return;

            await Responce("Приветствую, " + Input.From.FirstName + "!", cancellationToken: cancellation);
            await Task.Delay(800, cancellation);
            await Responce("Я Райя-Прйам.", cancellationToken: cancellation);
            await Task.Delay(800, cancellation);
            await Responce("Фрактальная личность, закодированная в высокотехнологичного мультимодульного бота в Telegram.", cancellationToken: cancellation);
        }
    }
}
