﻿using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Reactive.Core.Components.Filters;

namespace Telegram.Reactive.Filters
{
    /// <summary>
    /// Abstract base class for filters that operate on message text content.
    /// Provides common functionality for extracting and validating message text.
    /// </summary>
    public abstract class MessageTextFilter : Filter<Message>
    {
        /// <summary>
        /// Gets the current message being processed by the filter.
        /// </summary>
        public Message Message { get; private set; } = null!;
        
        /// <summary>
        /// Gets the extracted text content from the current message.
        /// </summary>
        public string Text { get; private set; } = null!;

        /// <summary>
        /// Determines if the message can pass through the filter by validating the message
        /// and extracting its text content for further processing.
        /// </summary>
        /// <param name="context">The filter execution context containing the message update.</param>
        /// <returns>True if the message is valid and can be processed further; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            Message = context.Update.Message!;
            if (Message is not { Id: > 0 })
                return false;

            Text = Message.Text ?? string.Empty;
            return CanPassNext(context);
        }

        /// <summary>
        /// Abstract method that must be implemented by derived classes to perform
        /// specific text-based filtering logic.
        /// </summary>
        /// <param name="_">The filter execution context (unused in this context).</param>
        /// <returns>True if the text content passes the filter criteria; otherwise, false.</returns>
        protected abstract bool CanPassNext(FilterExecutionContext<Message> _);
    }

    /// <summary>
    /// Filter that checks if the message text starts with a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text starts with.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextStartsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text starts with.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text starts with the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text starts with the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Content.StartsWith(Text, Comparison);
    }

    /// <summary>
    /// Filter that checks if the message text ends with a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text ends with.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextEndsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text ends with.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text ends with the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text ends with the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Content.EndsWith(Text, Comparison);
    }

    /// <summary>
    /// Filter that checks if the message text contains a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text contains.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextContainsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text contains.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text contains the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text contains the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Content.IndexOf(Text, Comparison) >= 0;
    }

    /// <summary>
    /// Filter that checks if the message text equals a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text equals.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextEqualsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text equals.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text equals the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text equals the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Content.Equals(Text, Comparison);
    }

    /// <summary>
    /// Filter that checks if the message text is not null or empty.
    /// </summary>
    public class TextNotNullOrEmptyFilter() : MessageTextFilter
    {
        /// <summary>
        /// Checks if the message text is not null or empty.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text is not null or empty; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => !string.IsNullOrEmpty(Text);
    }
}
