using Telegram.Bot.Types;
using Telegram.Reactive.Filters.Components;

namespace Telegram.Reactive.Filters
{
    /// <summary>
    /// Filter that checks if a message contains a mention of the bot or a specific user.
    /// Requires a <see cref="MessageHasEntityFilter"/> to be applied first to identify mention entities.
    /// </summary>
    public class MentionedFilter : Filter<Message>
    {
        /// <summary>
        /// The username to check for in the mention (null means check for bot's username).
        /// </summary>
        private readonly string? Mention;

        /// <summary>
        /// Initializes a new instance of the <see cref="MentionedFilter"/> class that checks for bot mentions.
        /// </summary>
        public MentionedFilter()
        {
            Mention = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MentionedFilter"/> class that checks for specific user mentions.
        /// </summary>
        /// <param name="mention">The username to check for in the mention.</param>
        public MentionedFilter(string mention)
        {
            Mention = mention;
        }

        /// <summary>
        /// Checks if the message contains a mention of the specified user or bot.
        /// This filter requires a <see cref="MessageHasEntityFilter"/> to be applied first
        /// to identify mention entities in the message.
        /// </summary>
        /// <param name="context">The filter execution context containing the message and completed filters.</param>
        /// <returns>True if the message contains the specified mention; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the bot username is null and no specific mention is provided.</exception>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            if (context.Input.Text == null)
                return false;

            string userName = Mention ?? context.BotInfo.User.Username ?? throw new ArgumentNullException(nameof(context), "MentionedFilter requires BotInfo to be initialized");
            MessageEntity entity = context.CompletedFilters.Get<MessageHasEntityFilter>(0).FoundEntities.ElementAt(0);

            string mention = context.Input.Text.Substring(entity.Offset + 1, entity.Length - 1);
            return userName == mention;
        }
    }
}
