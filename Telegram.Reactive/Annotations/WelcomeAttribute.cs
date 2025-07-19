using Telegram.Bot.Types.Enums;
using Telegram.Reactive.Filters;

namespace Telegram.Reactive.Annotations
{
    /// <summary>
    /// Attribute for filtering message with command "start" in bot's private chats.
    /// Allows handlers to respond to "welcome" bot commands.
    /// </summary>
    public class WelcomeAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Creates new instance of <see cref="WelcomeAttribute"/>
        /// </summary>
        public WelcomeAttribute() : base(new MessageChatTypeFilter(Telegram.Bot.Types.Enums.ChatType.Private), new CommandAlliasFilter("start"))
        { }
    }
}
