using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Attributes.Components;
using Telegram.Reactive.Filters.Components;

namespace Telegram.Reactive.MadiatorCore.Descriptors
{
    /// <summary>
    /// Provides methods for inspecting handler types and retrieving their attributes and filters.
    /// </summary>
    public static class HandlerInspector
    {
        /// <summary>
        /// Gets the handler attribute from the specified member info.
        /// </summary>
        /// <param name="handlerType">The member info representing the handler type.</param>
        /// <returns>The handler attribute.</returns>
        public static UpdateHandlerAttributeBase GetHandlerAttribute(MemberInfo handlerType)
        {
            // Getting polling handler attribute
            IEnumerable<UpdateHandlerAttributeBase> handlerAttrs = handlerType.GetCustomAttributes<UpdateHandlerAttributeBase>();

            //
            return handlerAttrs.Single();
        }

        /// <summary>
        /// Gets the state keeper attribute from the specified member info, if present.
        /// </summary>
        /// <param name="handlerType">The member info representing the handler type.</param>
        /// <returns>The state keeper attribute, or null if not present.</returns>
        public static StateKeeperAttributeBase? GetStateKeeperAttribute(MemberInfo handlerType)
        {
            // Getting polling handler attribute
            IEnumerable<StateKeeperAttributeBase> handlerAttrs = handlerType.GetCustomAttributes<StateKeeperAttributeBase>();

            //
            return handlerAttrs.Any() ? handlerAttrs.Single() : null;
        }

        /// <summary>
        /// Gets all filter attributes for the specified handler type and update type.
        /// </summary>
        /// <param name="handlerType">The member info representing the handler type.</param>
        /// <param name="validUpdType">The valid update type.</param>
        /// <returns>An enumerable of filter attributes.</returns>
        public static IEnumerable<IFilter<Update>> GetFilterAttributes(MemberInfo handlerType, UpdateType validUpdType)
        {
            //
            IEnumerable<UpdateFilterAttributeBase> filters = handlerType.GetCustomAttributes<UpdateFilterAttributeBase>();

            //
            if (filters.Any(filterAttr => !filterAttr.AllowedTypes.Contains(validUpdType)))
                throw new InvalidOperationException();

            UpdateFilterAttributeBase? lastFilterAttribute = null;
            foreach (UpdateFilterAttributeBase filterAttribute in filters)
            {
                if (lastFilterAttribute != null)
                    lastFilterAttribute = filterAttribute.ProcessModifiers(lastFilterAttribute) ? lastFilterAttribute : null;

                if (lastFilterAttribute != null)
                {
                    if (filterAttribute.ProcessModifiers(lastFilterAttribute))
                    {
                        lastFilterAttribute = filterAttribute;
                        continue;
                    }
                    else
                    {
                        lastFilterAttribute = null;
                        yield return filterAttribute.AnonymousFilter;
                    }
                }
                else
                {
                    yield return filterAttribute.AnonymousFilter;
                }
            }
        }
    }
}
