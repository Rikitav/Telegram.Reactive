using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Components.Handlers.Building;
using Telegram.Reactive.Core.Components.StateKeeping;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Handlers.Building;
using Telegram.Reactive.Providers;
using Telegram.Reactive.StateKeeping;

namespace Telegram.Reactive
{
    /// <summary>
    /// Extension methods for reflection operations.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        /// <summary>
        /// Checks if a type implements the <see cref="ICustomDescriptorsProvider"/> interface.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type implements ICustomDescriptorsProvider; otherwise, false.</returns>
        public static bool IsCustomDescriptorsProvider(this Type type)
            => type.GetInterface(nameof(ICustomDescriptorsProvider)) != null;
    }

    /// <summary>
    /// Extension methods for handler containers.
    /// Provides convenient methods for creating awaiter builders.
    /// </summary>
    public static class HandlerContainerExtensions
    {
        /// <summary>
        /// Creates an awaiter builder for a specific update type.
        /// </summary>
        /// <typeparam name="TUpdate">The type of update to await.</typeparam>
        /// <param name="container">The handler container.</param>
        /// <param name="updateType">The type of update to await.</param>
        /// <returns>An awaiter builder for the specified update type.</returns>
        public static AwaiterHandlerBuilder<TUpdate> AwaitUpdate<TUpdate>(this IHandlerContainer container, UpdateType updateType) where TUpdate : class
        {
            return new AwaiterHandlerBuilder<TUpdate>(updateType, container.HandlingUpdate, container.AwaitingProvider);
        }

        /// <summary>
        /// Creates an awaiter builder for any update type.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <returns>An awaiter builder for any update type.</returns>
        public static AwaiterHandlerBuilder<Update> AwaitAny(this IHandlerContainer container)
        {
            return new AwaiterHandlerBuilder<Update>(UpdateType.Unknown, container.HandlingUpdate, container.AwaitingProvider);
        }

        /// <summary>
        /// Creates an awaiter builder for message updates.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <returns>An awaiter builder for message updates.</returns>
        public static AwaiterHandlerBuilder<Message> AwaitMessage(this IHandlerContainer container)
        {
            return new AwaiterHandlerBuilder<Message>(UpdateType.Message, container.HandlingUpdate, container.AwaitingProvider);
        }

        /// <summary>
        /// Creates an awaiter builder for callback query updates.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <returns>An awaiter builder for callback query updates.</returns>
        public static AwaiterHandlerBuilder<CallbackQuery> AwaitCallbackQuery(this IHandlerContainer container)
        {
            return new AwaiterHandlerBuilder<CallbackQuery>(UpdateType.CallbackQuery, container.HandlingUpdate, container.AwaitingProvider);
        }

        /// <summary>
        /// Gets a state keeper instance for the specified types.
        /// </summary>
        /// <typeparam name="TKey">The type of the state key.</typeparam>
        /// <typeparam name="TState">The type of the state value.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="_">The handler container (unused).</param>
        /// <returns>The state keeper instance.</returns>
        public static TKeeper GetStateKeeper<TKey, TState, TKeeper>(this IHandlerContainer _) where TKey : notnull where TState : IEquatable<TState> where TKeeper : StateKeeperBase<TKey, TState>, new()
            => StateKeeperAttribute<TKey, TState, TKeeper>.StateKeeper;
    }

    /// <summary>
    /// Extension methods for handler builders.
    /// Provides convenient methods for creating handlers and setting state keepers.
    /// </summary>
    public static partial class HandlerBuilderExtensions
    {
        /// <summary>
        /// Creates a handler builder for a specific update type.
        /// </summary>
        /// <typeparam name="TUpdate">The type of update to handle.</typeparam>
        /// <param name="handlers">The handlers collection.</param>
        /// <param name="updateType">The type of update to handle.</param>
        /// <returns>A handler builder for the specified update type.</returns>
        public static HandlerBuilder<TUpdate> CreateHandler<TUpdate>(this IHandlersCollection handlers, UpdateType updateType) where TUpdate : class
        {
            return new HandlerBuilder<TUpdate>(updateType, handlers);
        }

        /// <summary>
        /// Creates a handler builder for any update type.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <returns>A handler builder for any update type.</returns>
        public static HandlerBuilder<Update> CreateAny(this IHandlersCollection handlers)
        {
            return new HandlerBuilder<Update>(UpdateType.Unknown, handlers);
        }

        /// <summary>
        /// Creates a handler builder for message updates.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <returns>A handler builder for message updates.</returns>
        public static HandlerBuilder<Message> CreateMessage(this IHandlersCollection handlers)
        {
            return new HandlerBuilder<Message>(UpdateType.Message, handlers);
        }

        /// <summary>
        /// Creates a handler builder for callback query updates.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <returns>A handler builder for callback query updates.</returns>
        public static HandlerBuilder<CallbackQuery> CreateCallbackQuery(this IHandlersCollection handlers)
        {
            return new HandlerBuilder<CallbackQuery>(UpdateType.CallbackQuery, handlers);
        }

        /// <summary>
        /// Sets a numeric state keeper with a custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The numeric state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this IHandlerBuilderActions<TBuilder> handlerBuilder, int myState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            => handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(myState, keyResolver);

        /// <summary>
        /// Sets a numeric state keeper with a special state and custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this IHandlerBuilderActions<TBuilder> handlerBuilder, SpecialState specialState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            => handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(specialState, keyResolver);

        /// <summary>
        /// Sets a numeric state keeper with the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The numeric state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this IHandlerBuilderActions<TBuilder> handlerBuilder, int myState)
            where TBuilder : HandlerBuilderBase
            => handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(myState, new SenderIdResolver());

        /// <summary>
        /// Sets a numeric state keeper with a special state and the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this IHandlerBuilderActions<TBuilder> handlerBuilder, SpecialState specialState)
            where TBuilder : HandlerBuilderBase
            => handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(specialState, new SenderIdResolver());

        /// <summary>
        /// Sets an enum state keeper with a custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The enum state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this IHandlerBuilderActions<TBuilder> handlerBuilder, TEnum myState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
            => handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(myState, keyResolver);

        /// <summary>
        /// Sets an enum state keeper with a special state and custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this IHandlerBuilderActions<TBuilder> handlerBuilder, SpecialState specialState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
            => handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(specialState, keyResolver);

        /// <summary>
        /// Sets an enum state keeper with the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The enum state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this IHandlerBuilderActions<TBuilder> handlerBuilder, TEnum myState)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
            => handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(myState, new SenderIdResolver());

        /// <summary>
        /// Sets an enum state keeper with a special state and the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this IHandlerBuilderActions<TBuilder> handlerBuilder, SpecialState specialState)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
            => handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(specialState, new SenderIdResolver());
    }
}
