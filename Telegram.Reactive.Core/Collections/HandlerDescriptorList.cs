using System.Collections;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Configuration;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Providers;

namespace Telegram.Reactive.Core.Collections
{
    /// <summary>
    /// The collection containing the <see cref="HandlerDescriptor"/>'s. Used to route <see cref="Update"/>'s in <see cref="IHandlersProvider"/>
    /// </summary>
    public sealed class HandlerDescriptorList : IEnumerable<HandlerDescriptor>
    {
        private readonly SortedList<DescriptorIndexer, HandlerDescriptor> _innerCollection;
        private readonly IHandlersCollectingOptions? _options;
        private readonly UpdateType _handlingType;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly { get; private set; } = false;

        /// <summary>
        /// Gets the <see cref="UpdateType"/> of handlers in this collection.
        /// </summary>
        public UpdateType HandlingType => _handlingType;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptorList"/> class without a specific <see cref="UpdateType"/>.
        /// </summary>
        public HandlerDescriptorList()
            : this(UpdateType.Unknown, default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptorList"/> class.
        /// </summary>
        /// <param name="updateType">The update type for the handlers.</param>
        /// <param name="options">The collecting options.</param>
        public HandlerDescriptorList(UpdateType updateType, IHandlersCollectingOptions? options)
        {
            _innerCollection = [];
            _handlingType = updateType;
            _options = options;
        }

        /// <summary>
        /// Adds a new <see cref="HandlerDescriptor"/> to the collection.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to add.</param>
        /// <exception cref="CollectionFrozenException">Thrown if the collection is frozen.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the update type does not match.</exception>
        public void Add(HandlerDescriptor descriptor)
        {
            if (IsReadOnly)
                throw new CollectionFrozenException();

            if (_handlingType != UpdateType.Unknown && descriptor.UpdateType != _handlingType)
                throw new InvalidOperationException();

            while (_innerCollection.TryGetValue(descriptor.Indexer, out HandlerDescriptor? conflictDescriptor))
            {
                int koefficent = _options?.DescendConflictingPriority ?? false ? -1 : 1;
                int newPriority = conflictDescriptor.Indexer.Priority + koefficent;
                descriptor.UpdatePriority(newPriority);
            }

            _innerCollection.Add(descriptor.Indexer, descriptor);
        }

        /// <summary>
        /// Checks if the collection contains a <see cref="HandlerDescriptor"/> with the specified <see cref="DescriptorIndexer"/>.
        /// </summary>
        /// <param name="indexer">The descriptor indexer.</param>
        /// <returns>True if the descriptor exists; otherwise, false.</returns>
        public bool ContainsKey(DescriptorIndexer indexer)
        {
            return _innerCollection.ContainsKey(indexer);
        }

        /// <summary>
        /// Removes the <see cref="HandlerDescriptor"/> with the specified <see cref="DescriptorIndexer"/> from the collection.
        /// </summary>
        /// <param name="indexer">The descriptor indexer.</param>
        /// <returns>True if the descriptor was removed; otherwise, false.</returns>
        public bool Remove(DescriptorIndexer indexer)
        {
            return _innerCollection.Remove(indexer);
        }

        /// <summary>
        /// Freezes the <see cref="HandlerDescriptorList"/> and prohibits adding new elements to it.
        /// </summary>
        public void Freeze()
        {
            IsReadOnly = true;
        }

        /// <inheritdoc/>
        public IEnumerator<HandlerDescriptor> GetEnumerator()
        {
            return _innerCollection.Values.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerCollection.Values.GetEnumerator();
        }
    }
}
