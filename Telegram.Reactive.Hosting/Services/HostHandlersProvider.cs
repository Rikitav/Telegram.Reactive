using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Polling;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Providers;

namespace Telegram.Reactive.Hosting.Services
{
    public class HostHandlersProvider : HandlersProvider
    {
        private readonly IServiceProvider Services;
        private readonly ILogger<HostHandlersProvider> Logger;

        public HostHandlersProvider(IHandlersCollection handlers, IOptions<TelegramBotOptions> options, ITelegramBotInfo botInfo, IServiceProvider serviceProvider, ILogger<HostHandlersProvider> logger)
            : base(handlers.Values, options.Value, botInfo)
        {
            Services = serviceProvider;
            Logger = logger;
            handlers.Values.ForEach(list => list.Freeze());
        }

        public override IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update)
        {
            IEnumerable<DescribedHandlerInfo> handlers = base.GetHandlers(updateRouter, client, update).ToArray();
            Logger.LogInformation("Described handlers : {0}", string.Join(", ", handlers.Select(hndlr => hndlr.DisplayString ?? hndlr.HandlerInstance.GetType().Name)));
            return handlers;
        }

        public override UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor)
        {
            IServiceScope scope = Services.CreateScope();
            object handlerInstance = descriptor.ServiceKey == null
                ? scope.ServiceProvider.GetRequiredService(descriptor.HandlerType)
                : scope.ServiceProvider.GetRequiredKeyedService(descriptor.HandlerType, descriptor.ServiceKey);

            if (handlerInstance is not UpdateHandlerBase updateHandler)
                throw new InvalidOperationException();

            updateHandler.LifetimeToken.OnLifetimeEnded += _ => scope.Dispose();
            return updateHandler;
        }
    }
}
