namespace Telegram.Reactive.Handlers.Building.Components
{
    /// <summary>
    /// Defines a builder for regular handler logic for a specific update type.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update to handle.</typeparam>
    public interface IRegularHandlerBuilder<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// Builds the handler logic using the specified execution delegate.
        /// </summary>
        /// <param name="executeHandler">The delegate to execute the handler logic.</param>
        public void Build(AbstractHandlerAction<TUpdate> executeHandler);
    }
}
