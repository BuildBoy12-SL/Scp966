// -----------------------------------------------------------------------
// <copyright file="AnnounceTermination.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966.Patches
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using HarmonyLib;
    using PlayerStatsSystem;

    /// <summary>
    /// Patches <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination"/> to allow Scp966 players have a unique death announcement.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnounceTermination
    {
        private static readonly Role Scp966Role = new Role { fullName = "SCP-966" };

        private static bool Prefix(ReferenceHub scp, DamageHandlerBase hit)
        {
            if (!Plugin.Instance.Config.Scp966.Check(Player.Get(scp)))
                return true;

            string announcement = hit.CassieDeathAnnouncement.Announcement;
            if (string.IsNullOrEmpty(announcement))
                return false;

            NineTailedFoxAnnouncer.scpDeaths.Add(new NineTailedFoxAnnouncer.ScpDeath
            {
                scpSubjects = new List<Role> { Scp966Role },
                announcement = announcement,
                subtitleParts = hit.CassieDeathAnnouncement.SubtitleParts,
            });

            Log.Debug($"Announcing Scp966 for player [{scp.nicknameSync.Network_myNickSync}]", Plugin.Instance.Config.Debug);
            return false;
        }
    }
}