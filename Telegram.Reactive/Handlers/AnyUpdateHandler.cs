﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.Core.Components.Filters;

namespace Telegram.Reactive.Handlers
{
    /// <summary>
    /// Attribute that marks a handler to process any type of update.
    /// This handler will be triggered for all incoming updates regardless of their type.
    /// </summary>
    /// <param name="concurrency">The maximum number of concurrent executions allowed (default: -1 for unlimited).</param>
    public class AnyUpdateHandlerAttribute(int concurrency = -1) : UpdateHandlerAttribute<AnyUpdateHandler>(UpdateType.Unknown, concurrency)
    {
        /// <summary>
        /// Always returns true, allowing any update to pass through this filter.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>Always returns true to allow any update.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context) => true;
    }

    /// <summary>
    /// Abstract base class for handlers that can process any type of update.
    /// Provides a foundation for creating handlers that respond to all incoming updates.
    /// </summary>
    public abstract class AnyUpdateHandler() : AbstractUpdateHandler<Update>(UpdateType.Unknown)
    {

    }
}
