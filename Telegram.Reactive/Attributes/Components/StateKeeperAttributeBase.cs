using Telegram.Bot.Types;
using Telegram.Reactive.Filters.Components;
using Telegram.Reactive.Handlers.Components;
using Telegram.Reactive.StateKeeping.Components;

namespace Telegram.Reactive.Attributes.Components
{
    /// <summary>
    /// Sets the state in which the <see cref="UpdateHandlerBase"/> can be executed
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class StateKeeperAttributeBase : Attribute, IFilter<Update>
    {
        /// <inheritdoc/>
        public bool IsCollectible => this.HasPublicProperties();

        /// <summary>
        /// Creates a new instance <see cref="StateKeeperBase{TKey, TState}"/>
        /// </summary>
        /// <param name="stateKeeperType"></param>
        /// <exception cref="ArgumentException"></exception>
        protected StateKeeperAttributeBase(Type stateKeeperType)
        {
            if (!stateKeeperType.IsAssignableToGenericType(typeof(StateKeeperBase<,>)))
                throw new ArgumentException(stateKeeperType + " is not a StateKeeperBase", nameof(stateKeeperType));
        }

        /// <summary>
        /// Realizes a <see cref="IFilter{T}"/> for validation of the current <see cref="StateKeeperBase{TKey, TState}"/> in the polling routing
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract bool CanPass(FilterExecutionContext<Update> context);
    }
}
