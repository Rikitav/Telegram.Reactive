﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Providers;

namespace Telegram.Reactive.Core.Components.Handlers
{
    /// <summary>
    /// Interface for handler containers that provide context and resources for update handlers.
    /// Contains all necessary information and services that handlers need during execution.
    /// </summary>
    public interface IHandlerContainer
    {
        /// <summary>
        /// Gets the <see cref="Update"/> being handled.
        /// </summary>
        public Update HandlingUpdate { get; }

        /// <summary>
        /// Gets the <see cref="ITelegramBotClient"/> used for this handler.
        /// </summary>
        public ITelegramBotClient Client { get; }

        /// <summary>
        /// Gets the extra data associated with the handler execution.
        /// </summary>
        public Dictionary<string, object> ExtraData { get; }

        /// <summary>
        /// Gets the <see cref="CompletedFiltersList"/> for this handler.
        /// </summary>
        public CompletedFiltersList CompletedFilters { get; }

        /// <summary>
        /// Gets the <see cref="IAwaitingProvider"/> for awaiting operations.
        /// </summary>
        public IAwaitingProvider AwaitingProvider { get; }
    }
}
