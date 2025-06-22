using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Core.Components.Attributes;
using Telegram.Reactive.Core.Components.Filters;
using Telegram.Reactive.Core.Components.Handlers;
using Telegram.Reactive.Core.Descriptors;
using Telegram.Reactive.Core.Providers;
using Telegram.Reactive.Providers;

namespace Telegram.Reactive.Handlers
{
    /// <summary>
    /// Abstract base class for handlers that support branching execution based on different methods.
    /// Allows multiple handler methods to be defined in a single class, each with its own filters.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    public abstract class BranchingUpdateHandler<TUpdate> : AbstractUpdateHandler<TUpdate>, IHandlerContainerFactory, ICustomDescriptorsProvider where TUpdate : class
    {
        /// <summary>
        /// The method info for the current branch being executed.
        /// </summary>
        private MethodInfo? branchMethodInfo = null;

        /// <summary>
        /// Gets the binding flags used to discover branch methods.
        /// </summary>
        protected virtual BindingFlags BranchesBindingFlags => BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// Gets the allowed return types for branch methods.
        /// </summary>
        protected virtual Type[] AllowedBranchReturnTypes => [typeof(void), typeof(Task)];

        /// <summary>
        /// Gets the cancellation token for the current execution.
        /// </summary>
        protected CancellationToken Cancellation { get; private set; } = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchingUpdateHandler{TUpdate}"/> class.
        /// </summary>
        /// <param name="handlingUpdateType">The type of update this handler processes.</param>
        protected BranchingUpdateHandler(UpdateType handlingUpdateType)
            : base(handlingUpdateType) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BranchingUpdateHandler{TUpdate}"/> class with a specific branch method.
        /// </summary>
        /// <param name="handlingUpdateType">The type of update this handler processes.</param>
        /// <param name="branch">The specific branch method to execute.</param>
        protected BranchingUpdateHandler(UpdateType handlingUpdateType, MethodInfo branch)
            : base(handlingUpdateType) => branchMethodInfo = branch;

        /// <summary>
        /// Describes all handler branches in this class.
        /// </summary>
        /// <returns>A collection of handler descriptors for each branch method.</returns>
        /// <exception cref="Exception">Thrown when no branch methods are found.</exception>
        public IEnumerable<HandlerDescriptor> DescribeHandlers()
        {
            Type thisType = GetType();
            UpdateHandlerAttributeBase updateHandlerAttribute = HandlerInspector.GetHandlerAttribute(thisType);
            IEnumerable<IFilter<Update>> handlerFilters = HandlerInspector.GetFilterAttributes(thisType, HandlingUpdateType);

            MethodInfo[] handlerBranches = thisType.GetMethods().Where(branch => branch.DeclaringType == thisType).ToArray();
            //MethodInfo[] handlerBranches = thisType.GetMethods(BranchesBindingFlags);
            if (handlerBranches.Length == 0)
                throw new Exception();

            /*
            for (int i = 0; i < handlerBranches.Length; i++)
            {
                MethodInfo branch = handlerBranches[i];
                yield return DescribeBranch(branch, updateHandlerAttribute, handlerFilters);
            }
            */

            for (int i = 0; i < handlerBranches.Length; i++)
            {
                int index = i;
                MethodInfo branch = handlerBranches[index];
                yield return DescribeBranch(branch, updateHandlerAttribute, handlerFilters, () =>
                {
                    BranchingUpdateHandler<TUpdate> handler = (BranchingUpdateHandler<TUpdate>)Activator.CreateInstance(thisType);
                    handler.branchMethodInfo = branch;
                    return handler;
                });
            }
        }

        /// <summary>
        /// Describes a specific branch method.
        /// </summary>
        /// <param name="branch">The branch method to describe.</param>
        /// <param name="handlerAttribute">The handler attribute for the class.</param>
        /// <param name="handlerFilters">The filters applied to the class.</param>
        /// <param name="factory">The factory of handler.</param>
        /// <returns>A handler descriptor for the branch method.</returns>
        /// <exception cref="Exception">Thrown when the branch method has parameters or invalid return type.</exception>
        protected virtual HandlerDescriptor DescribeBranch(MethodInfo branch, UpdateHandlerAttributeBase handlerAttribute, IEnumerable<IFilter<Update>> handlerFilters, Func<UpdateHandlerBase> factory)
        {
            Type thisType = GetType();

            if (branch.GetParameters().Any())
                throw new Exception();

            if (!AllowedBranchReturnTypes.Any(branch.ReturnType.Equals))
                throw new Exception();

            List<IFilter<Update>> branchFiltersList = HandlerInspector.GetFilterAttributes(branch, HandlingUpdateType).ToList();
            branchFiltersList.AddRange(handlerFilters);

            try
            {
                handlerAttribute = HandlerInspector.GetHandlerAttribute(branch);
            }
            catch
            {

            }

            DescriptorFiltersSet filtersSet = new DescriptorFiltersSet(
                handlerAttribute,
                HandlerInspector.GetStateKeeperAttribute(branch),
                branchFiltersList.ToArray());

            return new HandlerDescriptor(DescriptorType.General, thisType, HandlingUpdateType, handlerAttribute.GetIndexer(), filtersSet, factory)
            {
                DisplayString = string.Format("{0}+{1}", thisType.Name, branch.Name)
            };
        }

        /// <summary>
        /// Creates a handler container for this branching handler.
        /// </summary>
        /// <param name="awaitingProvider">The awaiting provider for the container.</param>
        /// <param name="handlerInfo">The handler information.</param>
        /// <returns>A handler container for this branching handler.</returns>
        /// <exception cref="Exception">Thrown when the awaiting provider is not of the expected type.</exception>
        public override IHandlerContainer CreateContainer(IAwaitingProvider awaitingProvider, DescribedHandlerInfo handlerInfo)
        {
            if (awaitingProvider is not AwaitingProvider _awaitingProvider)
                throw new Exception();

            return new AbstractHandlerContainer<TUpdate>(_awaitingProvider, handlerInfo);
        }

        /// <summary>
        /// Executes the current branch method.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <exception cref="Exception">Thrown when no branch method is set.</exception>
        public override async Task Execute(AbstractHandlerContainer<TUpdate> container, CancellationToken cancellation)
        {
            if (branchMethodInfo is null)
                throw new Exception();

            Cancellation = cancellation;
            await BranchExecuteWrapper(container, branchMethodInfo);
        }

        /// <summary>
        /// Wraps the execution of a branch method, handling both void and Task return types.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="methodInfo">The method to execute.</param>
        protected virtual async Task BranchExecuteWrapper(AbstractHandlerContainer<TUpdate> container, MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(this, []);
                return;
            }
            else
            {
                object branchReturn = methodInfo.Invoke(this, []);
                if (branchReturn == null)
                    return;

                if (branchReturn is Task branchTask)
                    await branchTask;
            }
        }
    }
}
