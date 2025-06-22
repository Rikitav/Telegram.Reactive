using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Components.Attributes;
using Telegram.Reactive.Core.Components.Filters;
using Telegram.Reactive.Core.Components.Handlers;

namespace Telegram.Reactive.Core.Descriptors
{
    /// <summary>
    /// Specifies the type of handler descriptor.
    /// </summary>
    public enum DescriptorType
    {
        /// <summary>
        /// General handler descriptor.
        /// </summary>
        General,
        /// <summary>
        /// Keyed handler descriptor (uses a service key).
        /// </summary>
        Keyed,
        /// <summary>
        /// Implicit handler descriptor.
        /// </summary>
        Implicit,
        /// <summary>
        /// Singleton handler descriptor (single instance).
        /// </summary>
        Singleton
    }

    /// <summary>
    /// Describes a handler, its type, filters, and instantiation logic.
    /// </summary>
    public class HandlerDescriptor
    {
        /// <summary>
        /// The type of the descriptor.
        /// </summary>
        public DescriptorType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the handler.
        /// </summary>
        public Type HandlerType
        {
            get;
            private set;
        }

        /// <summary>
        /// The update type handled by this handler.
        /// </summary>
        public UpdateType UpdateType
        {
            get;
            private set;
        }

        /// <summary>
        /// The indexer for handler concurrency and priority.
        /// </summary>
        public DescriptorIndexer Indexer
        {
            get;
            private set;
        }

        /// <summary>
        /// The set of filters associated with this handler.
        /// </summary>
        public DescriptorFiltersSet Filters
        {
            get;
            private set;
        }

        /// <summary>
        /// The service key for keyed handlers.
        /// </summary>
        public object? ServiceKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Factory for creating handler instances.
        /// </summary>
        public Func<UpdateHandlerBase>? InstanceFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Singleton instance of the handler, if applicable.
        /// </summary>
        public UpdateHandlerBase? SingletonInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Display string for the handler (for debugging or logging).
        /// </summary>
        public string? DisplayString
        {
            get;
            set;
        }

        public HandlerDescriptor(DescriptorType descriptorType, Type handlerType)
        {
            UpdateHandlerAttributeBase handlerAttribute = HandlerInspector.GetHandlerAttribute(handlerType);
            if (handlerAttribute.ExpectingHandlerType != null && !handlerAttribute.ExpectingHandlerType.Contains(handlerType.BaseType))
                throw new ArgumentException();

            StateKeeperAttributeBase? stateKeeperAttribute = HandlerInspector.GetStateKeeperAttribute(handlerType);
            IFilter<Update>[] filters = HandlerInspector.GetFilterAttributes(handlerType, handlerAttribute.Type).ToArray();

            Type = descriptorType;
            HandlerType = handlerType;
            UpdateType = handlerAttribute.Type;
            Indexer = handlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(handlerAttribute, stateKeeperAttribute, filters);
        }

        public HandlerDescriptor(Type handlerType, object serviceKey) : this(DescriptorType.Keyed, handlerType)
        {
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            SingletonInstance = singletonInstance ?? throw new ArgumentNullException(nameof(singletonInstance));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            SingletonInstance = singletonInstance ?? throw new ArgumentNullException(nameof(singletonInstance));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            SingletonInstance = singletonInstance ?? throw new ArgumentNullException(nameof(singletonInstance));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        public void UpdatePriority(int newPriority)
        {
            Indexer = Indexer.UpdatePriority(newPriority);
        }
    }
}
