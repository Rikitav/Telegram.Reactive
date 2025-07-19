using Telegram.Reactive.MadiatorCore;
using Telegram.Reactive.MadiatorCore.Descriptors;

namespace Telegram.Reactive.Handlers.Components
{
    /// <summary>
    /// Factory interface for creating handler containers.
    /// Provides a way to create handler containers with specific providers and handler information.
    /// </summary>
    public interface IHandlerContainerFactory
    {
        /// <summary>
        /// Creates a new <see cref="IHandlerContainer"/> for the specified awaiting provider and handler info.
        /// </summary>
        /// <param name="awaitingProvider">The <see cref="IAwaitingProvider"/> to use.</param>
        /// <param name="handlerInfo">The <see cref="DescribedHandlerInfo"/> for the handler.</param>
        /// <returns>A new <see cref="IHandlerContainer"/> instance.</returns>
        public IHandlerContainer CreateContainer(IAwaitingProvider awaitingProvider, DescribedHandlerInfo handlerInfo);
    }
}
