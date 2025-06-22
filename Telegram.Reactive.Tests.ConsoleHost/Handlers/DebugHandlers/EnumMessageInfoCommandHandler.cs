using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.DebugHandlers
{
    [CommandHandler, CommandAllias("info"), MessageReplied, IsDebugEnvironment]
    public class EnumMessageInfoCommandHandler : CommandHandler
    {
        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            foreach (PropertyInfo prop in Input.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                object? propValue = prop.GetValue(Input);
                if (propValue == null)
                    continue;

                string propFormat = string.Format("Name: {0},\nType: {1},\nValue: {2}.", prop.Name, prop.PropertyType, propValue.ToString());
                await Responce(propFormat, cancellationToken: cancellation);
            }
        }
    }
}
