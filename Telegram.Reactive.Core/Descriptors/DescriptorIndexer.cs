using Telegram.Reactive.Core.Components.Attributes;

namespace Telegram.Reactive.Core.Descriptors
{
    /// <summary>
    /// Represents an indexer for handler descriptors, containing concurrency and priority information.
    /// </summary>
    public readonly struct DescriptorIndexer(int concurrency, int priority) : IComparable<DescriptorIndexer>
    {
        /// <summary>
        /// The concurrency level for the handler.
        /// </summary>
        public readonly int Concurrency = concurrency;
        /// <summary>
        /// The priority of the handler.
        /// </summary>
        public readonly int Priority = priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorIndexer"/> struct from a handler attribute.
        /// </summary>
        /// <param name="pollingHandler">The handler attribute.</param>
        public DescriptorIndexer(UpdateHandlerAttributeBase pollingHandler)
            : this(pollingHandler.Concurrency, pollingHandler.Priority) { }

        /// <summary>
        /// Returns a new <see cref="DescriptorIndexer"/> with updated priority.
        /// </summary>
        /// <param name="priority">The new priority value.</param>
        /// <returns>A new <see cref="DescriptorIndexer"/> instance.</returns>
        public DescriptorIndexer UpdatePriority(int priority)
            => new DescriptorIndexer(Concurrency, priority);

        /// <summary>
        /// Returns a new <see cref="DescriptorIndexer"/> with updated concurrency.
        /// </summary>
        /// <param name="concurrency">The new concurrency value.</param>
        /// <returns>A new <see cref="DescriptorIndexer"/> instance.</returns>
        public DescriptorIndexer UpdateConcurrency(int concurrency)
            => new DescriptorIndexer(concurrency, Priority);

        /// <summary>
        /// Compares this instance to another <see cref="DescriptorIndexer"/>.
        /// </summary>
        /// <param name="other">The other indexer to compare to.</param>
        /// <returns>An integer indicating the relative order.</returns>
        public int CompareTo(DescriptorIndexer other)
        {
            int concurrencyCmp = Concurrency.CompareTo(other.Concurrency);
            return concurrencyCmp != 0 ? concurrencyCmp : Priority.CompareTo(other.Priority);
        }

        /// <summary>
        /// Returns a string representation of the indexer.
        /// </summary>
        /// <returns>A string in the format (C:concurrency, P:priority).</returns>
        public override string ToString()
        {
            return string.Format("(C:{0}, P:{1})", Concurrency, Priority);
        }
    }
}
