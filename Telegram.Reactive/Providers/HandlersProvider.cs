﻿using System.Collections.ObjectModel;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Components.Filters;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Polling;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.FilterAttributes;

namespace Telegram.Reactive.Providers
{
    /// <summary>
    /// Provides handler resolution and instantiation logic for Telegram bot updates.
    /// Responsible for mapping update types to handler descriptors, filtering handlers based on update context,
    /// and creating handler instances with appropriate lifecycle management.
    /// </summary>
    public class HandlersProvider : IHandlersProvider
    {
        /// <summary>
        /// Read-only dictionary mapping <see cref="UpdateType"/> to lists of handler descriptors.
        /// Each descriptor list is frozen to prevent modification after initialization.
        /// </summary>
        protected readonly ReadOnlyDictionary<UpdateType, HandlerDescriptorList> HandlersDictionary;

        /// <summary>
        /// Configuration options for the bot and handler execution behavior.
        /// </summary>
        protected readonly TelegramBotOptions Options;

        /// <summary>
        /// Information about the Telegram bot instance, used for filter context creation.
        /// </summary>
        protected readonly ITelegramBotInfo BotInfo;

        /// <summary>
        /// Initializes a new instance of <see cref="HandlersProvider"/> with the specified handler collections and configuration.
        /// </summary>
        /// <param name="handlers">Collection of handler descriptor lists organized by update type</param>
        /// <param name="options">Configuration options for the bot and handler execution</param>
        /// <param name="botInfo">Information about the Telegram bot instance</param>
        /// <exception cref="ArgumentNullException">Thrown when options or botInfo is null</exception>
        public HandlersProvider(IEnumerable<HandlerDescriptorList> handlers, TelegramBotOptions options, ITelegramBotInfo botInfo)
        {
            HandlersDictionary = handlers.ForEach(list => list.Freeze()).ToReadOnlyDictionary(list => list.HandlingType);
            Options = options ?? throw new ArgumentNullException(nameof(options));
            BotInfo = botInfo ?? throw new ArgumentNullException(nameof(botInfo));
        }

        /// <summary>
        /// Gets the handlers that match the specified update, using the provided router and client.
        /// Searches for handlers by update type, falling back to Unknown type if no specific handlers are found.
        /// </summary>
        /// <param name="updateRouter">The update router for handler execution</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <returns>A collection of described handler information for the update</returns>
        public virtual IEnumerable<DescribedHandlerInfo> GetHandlers(IUpdateRouter updateRouter, ITelegramBotClient client, Update update)
        {
            if (!HandlersDictionary.TryGetValue(update.Type, out HandlerDescriptorList? descriptors))
            {
                if (!HandlersDictionary.TryGetValue(UpdateType.Unknown, out descriptors))
                    return [];
            }

            if (descriptors == null || !descriptors.Any())
                return [];

            return DescribeDescriptors(descriptors, updateRouter, client, update);
        }

        /// <summary>
        /// Describes all handler descriptors for a given update context.
        /// Processes descriptors in reverse order and respects the ExecuteOnlyFirstFoundHanlder option.
        /// </summary>
        /// <param name="descriptors">The list of handler descriptors to process</param>
        /// <param name="updateRouter">The update router for handler execution</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <returns>A collection of described handler information</returns>
        public virtual IEnumerable<DescribedHandlerInfo> DescribeDescriptors(HandlerDescriptorList descriptors, IUpdateRouter updateRouter, ITelegramBotClient client, Update update)
        {
            foreach (HandlerDescriptor descriptor in descriptors.Reverse())
            {
                DescribedHandlerInfo? describedHandler = DescribeHandler(descriptor, updateRouter, client, update);
                if (describedHandler == null)
                    continue;

                yield return describedHandler;
                if (Options.ExecuteOnlyFirstFoundHanlder)
                    break;
            }
        }

        /// <summary>
        /// Describes a single handler descriptor for a given update context.
        /// Validates the handler's filters against the update and creates a handler instance if validation passes.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to process</param>
        /// <param name="updateRouter">The update router for handler execution</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <returns>The described handler info if validation passes; otherwise, null</returns>
        public virtual DescribedHandlerInfo? DescribeHandler(HandlerDescriptor descriptor, IUpdateRouter updateRouter, ITelegramBotClient client, Update update)
        {
            FilterExecutionContext<Update> filterContext = new FilterExecutionContext<Update>(BotInfo, update, update);
            if (!descriptor.Filters.Validate(filterContext))
                return null;

            UpdateHandlerBase handlerInstance = GetHandlerInstance(descriptor);
            return new DescribedHandlerInfo(updateRouter, client, handlerInstance, filterContext, descriptor.DisplayString);
        }

        /// <summary>
        /// Instantiates a handler for the given descriptor, using the appropriate creation strategy based on descriptor type.
        /// Supports singleton, implicit, keyed, and general descriptor types with different instantiation patterns.
        /// </summary>
        /// <param name="descriptor">The handler descriptor containing type and instantiation information</param>
        /// <returns>An instance of <see cref="UpdateHandlerBase"/> for the descriptor</returns>
        /// <exception cref="Exception">Thrown when the descriptor type is not recognized</exception>
        public virtual UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor)
        {
            switch (descriptor.Type)
            {
                case DescriptorType.Implicit:
                case DescriptorType.Singleton:
                    {
                        return descriptor.SingletonInstance ??= (descriptor.InstanceFactory != null
                            ? descriptor.SingletonInstance = descriptor.InstanceFactory.Invoke()
                            : descriptor.SingletonInstance = (UpdateHandlerBase)Activator.CreateInstance(descriptor.HandlerType, [descriptor.UpdateType]));
                    }

                case DescriptorType.Keyed:
                case DescriptorType.General:
                    {
                        return descriptor.InstanceFactory == null
                            ? (UpdateHandlerBase)Activator.CreateInstance(descriptor.HandlerType, [descriptor.UpdateType])
                            : descriptor.InstanceFactory.Invoke();
                    }

                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Gets the list of bot commands defined by all handler types with <see cref="CommandAlliasAttribute"/>.
        /// Extracts command aliases and descriptions from message handlers for bot command registration.
        /// </summary>
        /// <returns>A collection of <see cref="BotCommand"/> objects for the bot</returns>
        public IEnumerable<BotCommand> GetBotCommands()
        {
            if (!HandlersDictionary.TryGetValue(UpdateType.Message, out HandlerDescriptorList? list))
                return [];

            return list
                .Select(descriptor => descriptor.HandlerType)
                .SelectMany(handlerType => handlerType.GetCustomAttributes<CommandAlliasAttribute>())
                .SelectMany(attribute => attribute.Alliases.Select(alias => new BotCommand(alias, attribute.Description)));
        }

        /// <summary>
        /// Determines whether the provider contains any handlers.
        /// </summary>
        /// <returns>True if there are no handlers registered; otherwise, false</returns>
        public virtual bool IsEmpty()
        {
            return HandlersDictionary.Count == 0;
        }
    }
}
