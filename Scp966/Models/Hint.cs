// -----------------------------------------------------------------------
// <copyright file="Hint.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966.Models
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// A wrapper to save hints to be shown to a player.
    /// </summary>
    [Serializable]
    public class Hint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        public Hint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        /// <param name="message"><inheritdoc cref="Message"/></param>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        /// <param name="show"><inheritdoc cref="Show"/></param>
        public Hint(string message, float duration, bool show = true)
        {
            Message = message;
            Duration = duration;
            Show = show;
        }

        /// <summary>
        /// Gets or sets the hint to be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the duration the hint should display.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hint should be displayed.
        /// </summary>
        public bool Show { get; set; }

        /// <summary>
        /// Displays the hint to the specified player.
        /// </summary>
        /// <param name="player">The player to display the hint to.</param>
        public void Display(Player player)
        {
            if (Show)
                player.ShowHint(Message, Duration);
        }
    }
}