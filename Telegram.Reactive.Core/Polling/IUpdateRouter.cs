using Telegram.Bot.Polling;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Providers;

namespace Telegram.Reactive.Core.Polling
{
    /// <summary>
    /// Interface for update routers that handle incoming updates and manage handler execution.
    /// Combines update handling capabilities with polling provider functionality and exception handling.
    /// </summary>
    public interface IUpdateRouter : IUpdateHandler, IPollingProvider
    {   
        /// <summary>
        /// Gets the <see cref="TelegramBotOptions"/> for the router.
        /// </summary>
        public TelegramBotOptions Options { get; }
        
        /// <summary>
        /// Gets the <see cref="IUpdateHandlersPool"/> that manages handler execution.
        /// </summary>
        public IUpdateHandlersPool HandlersPool { get; }

        /// <summary>
        /// Gets or sets the <see cref="IRouterExceptionHandler"/> for handling exceptions.
        /// </summary>
        public IRouterExceptionHandler? ExceptionHandler { get; set; }
    }
}
