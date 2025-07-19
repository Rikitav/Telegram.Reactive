using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Handlers.Building.Components;
using Telegram.Reactive.MadiatorCore.Descriptors;

namespace Telegram.Reactive.MadiatorCore
{
    /// <summary>
    /// Provider for managing awaiting handlers that can wait for specific update types.
    /// </summary>
    public interface IAwaitingProvider : IHandlersProvider
    {
        /// <summary>
        /// Registers the usage of a handler and returns a disposable object to manage its lifetime.
        /// </summary>
        /// <param name="handlerDescriptor">The <see cref="HandlerDescriptor"/> to use.</param>
        /// <returns>An <see cref="IDisposable"/> that manages the handler's usage lifetime.</returns>
        public IDisposable UseHandler(HandlerDescriptor handlerDescriptor);
    }
}
