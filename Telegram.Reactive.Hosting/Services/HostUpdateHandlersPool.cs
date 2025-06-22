using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Polling;

namespace Telegram.Reactive.Hosting.Services
{
    public class HostUpdateHandlersPool(IOptions<TelegramBotOptions> options, ILogger<HostUpdateHandlersPool> logger)
        : UpdateHandlersPool(options.Value, options.Value.GlobalCancellationToken)
    {
        private readonly ILogger<HostUpdateHandlersPool> _logger = logger;

        protected override async Task ExecuteHandlerWrapper(DescribedHandlerInfo enqueuedHandler)
        {
            _logger.LogInformation("Handler \"{0}\" has entered execution pool", enqueuedHandler.DisplayString);
            await base.ExecuteHandlerWrapper(enqueuedHandler);
        }
    }
}
