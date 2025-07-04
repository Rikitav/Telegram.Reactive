﻿using System.Globalization;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Attributes;
using Telegram.Reactive.Core.Collections;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.FilterAttributes;

namespace Telegram.Reactive.Providers
{
    /// <summary>
    /// Collection class for managing handler descriptors organized by update type.
    /// Provides functionality for collecting, adding, and organizing handlers.
    /// </summary>
    /// <param name="options">Optional configuration options for handler collecting.</param>
    public class HandlersCollection(IHandlersCollectingOptions? options) : IHandlersCollection
    {
        /// <summary>
        /// Dictionary that organizes handler descriptors by update type.
        /// </summary>
        protected readonly Dictionary<UpdateType, HandlerDescriptorList> InnerDictionary = [];
        
        /// <summary>
        /// Configuration options for handler collecting.
        /// </summary>
        protected readonly IHandlersCollectingOptions? Options = options;
        
        /// <summary>
        /// Gets whether handlers must have a parameterless constructor.
        /// </summary>
        protected virtual bool MustHaveParameterlessCtor => true;
        
        /// <summary>
        /// List of command aliases that have been registered.
        /// </summary>
        public readonly List<string> CommandAliasses = [];

        /// <inheritdoc/>
        public IEnumerable<UpdateType> Keys
        {
            get => InnerDictionary.Keys;
        }

        /// <inheritdoc/>
        public IEnumerable<HandlerDescriptorList> Values
        {
            get => InnerDictionary.Values;
        }

        /// <inheritdoc/>
        public HandlerDescriptorList this[UpdateType updateType]
        {
            get => InnerDictionary[updateType];
        }

        /// <inheritdoc/>
        /// <summary>
        /// Collects all handlers from the entry assembly domain-wide.
        /// Scans for types that implement handlers and adds them to the collection.
        /// </summary>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the entry assembly cannot be found.</exception>
        public virtual IHandlersCollection CollectHandlersDomainWide()
        {
            Assembly? entryAssembly = Assembly.GetEntryAssembly() ?? throw new Exception();
            entryAssembly.GetExportedTypes()
                .Where(type => type.GetCustomAttribute<DontCollectAttribute>() == null)
                .Where(type => type.IsHandlerRealization())
                .ForEach(type => AddHandler(type));

            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Adds a handler descriptor to the collection.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to add.</param>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the handler type doesn't have a parameterless constructor and MustHaveParameterlessCtor is true.</exception>
        public virtual IHandlersCollection AddDescriptor(HandlerDescriptor descriptor)
        {
            if (MustHaveParameterlessCtor && !descriptor.HandlerType.HasParameterlessCtor())
                throw new Exception();

            IntersectCommands(descriptor);
            GetDescriptorList(descriptor).Add(descriptor);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Adds a handler type to the collection.
        /// </summary>
        /// <typeparam name="THandler">The type of handler to add.</typeparam>
        /// <returns>This collection instance for method chaining.</returns>
        public virtual IHandlersCollection AddHandler<THandler>() where THandler : UpdateHandlerBase
        {
            AddHandler(typeof(THandler));
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Adds a handler type to the collection.
        /// </summary>
        /// <param name="handlerType">The type of handler to add.</param>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the type is not a valid handler implementation.</exception>
        public virtual IHandlersCollection AddHandler(Type handlerType)
        {
            if (!handlerType.IsHandlerRealization())
                throw new Exception();

            if (handlerType.IsCustomDescriptorsProvider())
            {
                foreach (HandlerDescriptor handlerDescriptor in InvokeCustomDescriptorsProvider(handlerType))
                    AddDescriptor(handlerDescriptor);

                return this;
            }

            HandlerDescriptor descriptor = new HandlerDescriptor(DescriptorType.General, handlerType);
            AddDescriptor(descriptor);
            return this;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Gets or creates a descriptor list for the specified update type.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to get the list for.</param>
        /// <returns>The descriptor list for the update type.</returns>
        public virtual HandlerDescriptorList GetDescriptorList(HandlerDescriptor descriptor)
        {
            if (!InnerDictionary.TryGetValue(descriptor.UpdateType, out HandlerDescriptorList? list))
            {
                list = new HandlerDescriptorList(descriptor.UpdateType, Options);
                InnerDictionary.Add(descriptor.UpdateType, list);
            }

            return list;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Checks for intersecting command aliases and handles them according to configuration.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to check for command aliases.</param>
        /// <exception cref="Exception">Thrown when intersecting command aliases are found and ExceptIntersectingCommandAliases is enabled.</exception>
        protected void IntersectCommands(HandlerDescriptor descriptor)
        {
            if (Options == null)
                return;

            if (!Options.ExceptIntersectingCommandAliases)
                return;

            CommandAlliasAttribute? alliasAttribute = descriptor.HandlerType.GetCustomAttribute<CommandAlliasAttribute>();
            if (alliasAttribute == null)
                return;

            if (CommandAliasses.Intersect(alliasAttribute.Alliases, StringComparer.InvariantCultureIgnoreCase).Any())
                throw new Exception(descriptor.HandlerType.FullName);

            CommandAliasses.AddRange(alliasAttribute.Alliases);
        }

        /// <summary>
        /// Invokes a custom descriptors provider to get handler descriptors.
        /// </summary>
        /// <param name="handlerType">The handler type that implements ICustomDescriptorsProvider.</param>
        /// <returns>A collection of handler descriptors from the custom provider.</returns>
        /// <exception cref="Exception">Thrown when the handler type doesn't have a parameterless constructor or cannot be instantiated.</exception>
        protected virtual IEnumerable<HandlerDescriptor> InvokeCustomDescriptorsProvider(Type handlerType)
        {
            if (!handlerType.HasParameterlessCtor())
                throw new Exception();

            ICustomDescriptorsProvider? provider = (ICustomDescriptorsProvider?)Activator.CreateInstance(handlerType);
            if (provider == null)
                throw new Exception();

            return provider.DescribeHandlers();
        }
    }
}
