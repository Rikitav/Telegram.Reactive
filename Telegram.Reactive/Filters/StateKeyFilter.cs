﻿using Telegram.Bot.Types;
using Telegram.Reactive.Core.Components.Filters;
using Telegram.Reactive.Core.Components.StateKeeping;

namespace Telegram.Reactive.Filters
{
    /// <summary>
    /// Filters updates by comparing a resolved state key with a target key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used for state resolution.</typeparam>
    public class StateKeyFilter<TKey> : Filter<Update> where TKey : IEquatable<TKey>
    {
        private readonly IStateKeyResolver<TKey> KeyResolver;
        private readonly TKey TargetKey;
        /// <summary>
        /// Initializes a new instance of the <see cref="StateKeyFilter{TKey}"/> class.
        /// </summary>
        /// <param name="keyResolver">The key resolver to extract the key from the update.</param>
        /// <param name="targetKey">The target key to compare with.</param>
        public StateKeyFilter(IStateKeyResolver<TKey> keyResolver, TKey targetKey)
        {
            KeyResolver = keyResolver;
            TargetKey = targetKey;
        }
        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<Update> context)
            => KeyResolver.ResolveKey(context.Input).Equals(TargetKey);
    }
}
