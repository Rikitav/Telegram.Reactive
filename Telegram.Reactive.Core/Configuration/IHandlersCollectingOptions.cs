using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.Reactive.Core.Configuration
{
    /// <summary>
    /// Interface for configuring handler collection behavior.
    /// Defines options that control how handlers are collected and processed during initialization.
    /// </summary>
    public interface IHandlersCollectingOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to descend the priority of conflicting handlers.
        /// </summary>
        public bool DescendConflictingPriority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to exclude intersecting command aliases.
        /// </summary>
        public bool ExceptIntersectingCommandAliases { get; set; }
    }
}
