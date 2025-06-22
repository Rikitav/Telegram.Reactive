using Telegram.Reactive.Core.Descriptors;

namespace Telegram.Reactive.Core.Providers
{
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
