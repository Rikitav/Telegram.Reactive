using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Polling;

namespace Telegram.Reactive.Core.Providers
{
    /// <summary>
    /// Provides methods to retrieve and describe handler information for updates.
    /// </summary>
    public interface IHandlersProvider
    {
        /// <summary>
        /// Gets the handlers for the specified update and context.
        /// </summary>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <returns>An enumerable of described handler info.</returns>
        public IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update);

        /// <summary>
        /// Describes all handler descriptors in the list for the given context.
        /// </summary>
        /// <param name="descriptors">The handler descriptor list.</param>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <returns>An enumerable of described handler info.</returns>
        public IEnumerable<DescribedHandlerInfo> DescribeDescriptors(HandlerDescriptorList descriptors, IUpdateRouter updateRouter, ITelegramBotClient client, Update update);

        /// <summary>
        /// Describes a single handler descriptor for the given context.
        /// </summary>
        /// <param name="descriptor">The handler descriptor.</param>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <returns>The described handler info, or null if not applicable.</returns>
        public DescribedHandlerInfo? DescribeHandler(HandlerDescriptor descriptor, IUpdateRouter updateRouter, ITelegramBotClient client, Update update);

        /// <summary>
        /// Gets an instance of the handler for the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The handler descriptor.</param>
        /// <returns>The handler instance.</returns>
        public UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor);

        /// <summary>
        /// Gets the list of bot commands supported by the provider.
        /// </summary>
        /// <returns>An enumerable of bot commands.</returns>
        public IEnumerable<BotCommand> GetBotCommands();

        /// <summary>
        /// Determines whether the provider contains any handlers.
        /// </summary>
        /// <returns>True if the provider is empty; otherwise, false.</returns>
        public bool IsEmpty();
    }
}
