using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Filters.Components;
using Telegram.Reactive.Handlers.Components;
using Telegram.Reactive.MadiatorCore;
using Telegram.Reactive.MadiatorCore.Descriptors;

namespace Telegram.Reactive.Handlers.Building.Components
{
    /// <summary>
    /// Base class for building handler descriptors and managing handler filters.
    /// </summary>
    public abstract class HandlerBuilderBase(Type buildingHandlerType, UpdateType updateType, IHandlersCollection? handlerCollection)
    {
        private static int HandlerServiceKeyIndex = 0;
        private readonly IHandlersCollection? HandlerCollection = handlerCollection;

        /// <summary>
        /// <see cref="UpdateType"/> of building handler
        /// </summary>
        protected readonly UpdateType UpdateType = updateType;
        
        /// <summary>
        /// Type of handler to build
        /// </summary>
        protected readonly Type BuildingHandlerType = buildingHandlerType;
        
        /// <summary>
        /// Filters applied to handler
        /// </summary>
        protected readonly List<IFilter<Update>> Filters = [];

        /// <summary>
        /// <see cref="DescriptorIndexer"/> of building handler
        /// </summary>
        protected DescriptorIndexer Indexer = new DescriptorIndexer(0, 0);
        
        /// <summary>
        /// Update validation filter of building handler
        /// </summary>
        protected IFilter<Update>? ValidateFilter;
        
        /// <summary>
        /// State keeper of building handler
        /// </summary>
        protected IFilter<Update>? StateKeeper;

        /// <summary>
        /// Builds an implicit <see cref="HandlerDescriptor"/> for the specified handler instance.
        /// </summary>
        /// <param name="instance">The <see cref="UpdateHandlerBase"/> instance.</param>
        /// <returns>The created <see cref="HandlerDescriptor"/>.</returns>
        protected HandlerDescriptor BuildImplicitDescriptor(UpdateHandlerBase instance)
        {
            object handlerServiceKey = GetImplicitHandlerServiceKey(BuildingHandlerType);

            HandlerDescriptor descriptor = new HandlerDescriptor(
                DescriptorType.Implicit, BuildingHandlerType,
                UpdateType, Indexer, ValidateFilter,
                Filters.ToArray(), StateKeeper,
                handlerServiceKey, instance);

            HandlerCollection?.AddDescriptor(descriptor);
            return descriptor;
        }

        /// <summary>
        /// Gets a unique service key for an implicit handler type.
        /// </summary>
        /// <param name="BuildingHandlerType">The handler type.</param>
        /// <returns>A unique service key string.</returns>
        public static object GetImplicitHandlerServiceKey(Type BuildingHandlerType)
            => string.Format("ImplicitHandler_{0}+{1}", HandlerServiceKeyIndex++, BuildingHandlerType.Name);
    }
}
