// -----------------------------------------------------------------------
// <copyright file="Chance.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966.Models
{
    using System;
    using Exiled.Loader;
    using UnityEngine;

    /// <summary>
    /// Represents a managed chance.
    /// </summary>
    [Serializable]
    public class Chance
    {
        private float chance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chance"/> class.
        /// </summary>
        public Chance()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chance"/> class.
        /// </summary>
        /// <param name="chance"><inheritdoc cref="Value"/></param>
        public Chance(float chance)
        {
            this.chance = chance;
        }

        /// <summary>
        /// Gets or sets the chance.
        /// </summary>
        public float Value
        {
            get => chance;
            set => chance = Mathf.Clamp(value, 0, 100);
        }

        /// <summary>
        /// Returns a value indicating whether the chance is met utilizing <see cref="System.Random"/>.
        /// </summary>
        /// <returns>Whether the chance was greater than the random value.</returns>
        public bool IsSuccess() => Loader.Random.Next(100) < chance;
    }
}