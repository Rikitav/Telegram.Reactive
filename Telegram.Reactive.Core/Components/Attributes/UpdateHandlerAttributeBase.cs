﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Components.Filters;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Descriptors;

namespace Telegram.Reactive.Core.Components.Attributes
{
    /// <summary>
    /// Defines the <see cref="UpdateType"/>'s and validator (<see cref="IFilter{T}"/>) of the <see cref="Update"/> that <see cref="UpdateHandlerBase"/> will process
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class UpdateHandlerAttributeBase : Attribute, IFilter<Update>
    {
        /// <inheritdoc/>
        public bool IsCollectible => this.HasPublicProperties();

        /// <summary>
        /// Gets an array of <see cref="UpdateHandlerBase"/> that this attribute can be attached to
        /// </summary>
        public Type[] ExpectingHandlerType { get; private set; }
        
        /// <summary>
        /// Gets an <see cref="UpdateType"/> that handlers processes
        /// </summary>
        public UpdateType Type { get; private set; }
        
        /// <summary>
        /// Gets or sets concurrency of this <see cref="UpdateHandlerBase"/> in same <see cref="UpdateType"/> pool
        /// </summary>
        public int Concurrency { get; set; }

        /// <summary>
        /// Gets or sets priority of this <see cref="UpdateHandlerBase"/> in same type handlers pool
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// Creates a new instance of <see cref="UpdateHandlerAttributeBase"/>
        /// </summary>
        /// <param name="expectingHandlerType"></param>
        /// <param name="updateType"></param>
        /// <param name="concurrency"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        protected internal UpdateHandlerAttributeBase(Type[] expectingHandlerType, UpdateType updateType, int concurrency = 0)
        {
            if (expectingHandlerType == null)
                throw new ArgumentNullException(nameof(expectingHandlerType));
            
            if (expectingHandlerType.Any(type => !type.IsHandlerAbstract()))
                throw new ArgumentException(nameof(expectingHandlerType));

            if (updateType == UpdateType.Unknown)
                throw new Exception();

            ExpectingHandlerType = expectingHandlerType;
            Type = updateType;
            Concurrency = concurrency;
        }

        /// <summary>
        /// Gets an <see cref="DescriptorIndexer"/> of this <see cref="UpdateHandlerAttributeBase"/> from <see cref="Concurrency"/> and <see cref="Priority"/>
        /// </summary>
        /// <returns></returns>
        public DescriptorIndexer GetIndexer()
            => new DescriptorIndexer(this);

        /// <summary>
        /// Validator (<see cref="IFilter{T}"/>) of the <see cref="Update"/> that <see cref="UpdateHandlerBase"/> will process
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract bool CanPass(FilterExecutionContext<Update> context);
    }
}
