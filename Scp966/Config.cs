// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966
{
    using Exiled.API.Interfaces;
    using Exiled.CustomRoles.API;
    using Scp966.Configs;
    using Scp966.CustomRoles;

    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug logs should be enabled.
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a configurable instance of the <see cref="Scp966Role"/> custom role.
        /// </summary>
        public Scp966Role Scp966 { get; set; } = new Scp966Role();

        /// <summary>
        /// Gets or sets a configurable instance of the <see cref="Configs.GoggledClasses"/> class.
        /// </summary>
        public GoggledClasses GoggledClasses { get; set; } = new GoggledClasses();

        /// <summary>
        /// Registers the custom roles defined in this config.
        /// </summary>
        public void Register() => Scp966?.Register();

        /// <summary>
        /// Unregisters the custom roles defined in this config.
        /// </summary>
        public void Unregister() => Scp966?.Unregister();
    }
}