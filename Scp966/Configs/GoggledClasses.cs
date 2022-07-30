// -----------------------------------------------------------------------
// <copyright file="GoggledClasses.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966.Configs
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Scp966.Models;

    /// <summary>
    /// A class to be configured to determine which users should be able to see Scp966 instances.
    /// </summary>
    public class GoggledClasses
    {
        /// <summary>
        /// Gets or sets a collection of roles paired with the chance that they can spawn with goggles.
        /// </summary>
        [Description("A collection of roles paired with the chance that they can spawn with goggles.")]
        public Dictionary<RoleType, Chance> Spawnable { get; set; } = new Dictionary<RoleType, Chance>
        {
            { RoleType.NtfPrivate, new Chance(10f) },
        };

        /// <summary>
        /// Gets or sets the broadcast that will be sent to players that spawn with goggles.
        /// </summary>
        [Description("The broadcast that will be sent to players that spawn with goggles.")]
        public Broadcast SpawnBroadcast { get; set; } = new Broadcast("You have spawned with <color=green>Goggles</color> that allow you to see <color=red>Scp966</color>!");

        /// <summary>
        /// Gets or sets a list of custom role names that cannot spawn with goggles.
        /// </summary>
        [Description("A list of custom role names that cannot spawn with goggles.")]
        public List<string> CustomRoleBlacklist { get; set; } = new List<string>
        {
            "CiSpy",
        };
    }
}