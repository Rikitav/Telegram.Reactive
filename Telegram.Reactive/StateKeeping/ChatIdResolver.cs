﻿using Telegram.Bot.Types;
using Telegram.Reactive.Core.Components.StateKeeping;

namespace Telegram.Reactive.StateKeeping
{
    /// <summary>
    /// Resolves chat ID from Telegram updates for state management purposes.
    /// Extracts the chat identifier from various types of updates to provide a consistent key for state operations.
    /// </summary>
    public class ChatIdResolver : IStateKeyResolver<long>
    {
        /// <summary>
        /// Resolves the chat ID from a Telegram update.
        /// </summary>
        /// <param name="keySource">The Telegram update to extract the chat ID from.</param>
        /// <returns>The chat ID as a long value.</returns>
        /// <exception cref="ArgumentException">Thrown when the update does not contain a valid chat ID.</exception>
        public long ResolveKey(Update keySource)
            => keySource.GetChatId() ?? throw new ArgumentException();
    }
}
