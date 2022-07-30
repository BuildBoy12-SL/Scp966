// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966
{
    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs;
    using Scp966.Models;

    /// <summary>
    /// General event handlers.
    /// </summary>
    public class EventHandlers
    {
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="plugin">The <see cref="Plugin{TConfig}"/> class reference.</param>
        public EventHandlers(Plugin plugin) => this.plugin = plugin;

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.IsAllowed || !plugin.Config.GoggledClasses.Spawnable.TryGetValue(ev.NewRole, out Chance chance))
                return;

            ev.Player.SessionVariables.Remove("Has966Goggles");

            foreach (string blacklistedRoleName in plugin.Config.GoggledClasses.CustomRoleBlacklist)
            {
                if (CustomRole.TryGet(blacklistedRoleName, out CustomRole blacklistedRole) &&
                    blacklistedRole.Check(ev.Player))
                    return;
            }

            if (!chance.IsSuccess())
                return;

            ev.Player.Broadcast(plugin.Config.GoggledClasses.SpawnBroadcast);
            ev.Player.SessionVariables.Add("Has966Goggles", true);
            foreach (Player player in plugin.Config.Scp966.TrackedPlayers)
                ev.Player.TargetGhostsHashSet.Remove(player.Id);
        }

        /// <summary>
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        /// Used as a precaution in case something is weird during fast round restarts.
        /// </summary>
        public void OnWaitingForPlayers()
        {
            foreach (Player player in Player.List)
            {
                player.SessionVariables.Remove("Has966Goggles");
                player.TargetGhostsHashSet.Clear();
            }
        }
    }
}