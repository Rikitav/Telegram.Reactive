using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Providers;

namespace Telegram.Reactive.Handlers.Building
{
    /// <summary>
    /// Internal handler used for awaiting specific update types.
    /// Provides synchronization mechanism for waiting for updates of a particular type.
    /// </summary>
    /// <param name="handlingUpdateType">The type of update this awaiter handler waits for.</param>
    internal class AwaiterHandler(UpdateType handlingUpdateType) : UpdateHandlerBase(handlingUpdateType), IHandlerContainerFactory, IDisposable
    {
        /// <summary>
        /// Manual reset event used for synchronization.
        /// </summary>
        private ManualResetEventSlim ResetEvent = new ManualResetEventSlim(false);
        
        /// <summary>
        /// Gets the update that triggered this awaiter handler.
        /// </summary>
        public Update HandlingUpdate { get; private set; } = null!;

        /// <summary>
        /// Waits for the specified update type to be received.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
        public void Wait(CancellationToken cancellationToken)
        {
            ResetEvent.Reset();
            ResetEvent.Wait(cancellationToken);
        }

        /// <summary>
        /// Creates a handler container for this awaiter handler.
        /// </summary>
        /// <param name="_">The awaiting provider (unused).</param>
        /// <param name="describedHandler">The handler information containing the update.</param>
        /// <returns>An empty handler container.</returns>
        public IHandlerContainer CreateContainer(IAwaitingProvider _, DescribedHandlerInfo describedHandler)
        {
            HandlingUpdate = describedHandler.HandlingUpdate;
            return new EmptyHandlerContainer();
        }

        /// <summary>
        /// Executes the awaiter handler by setting the reset event.
        /// </summary>
        /// <param name="container">The handler container (unused).</param>
        /// <param name="cancellation">The cancellation token (unused).</param>
        /// <returns>A completed task.</returns>
        protected override Task Execute(IHandlerContainer container, CancellationToken cancellation)
        {
            ResetEvent.Set();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes of the reset event.
        /// </summary>
        public void Dispose()
        {
            ResetEvent.Dispose();
            ResetEvent = null!;
        }
    }
}
