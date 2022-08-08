// -----------------------------------------------------------------------
// <copyright file="Scp966Role.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966.CustomRoles
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Events.EventArgs;
    using MEC;
    using MonoMod.Utils;
    using PlayerStatsSystem;
    using Scp966.Models;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <inheritdoc />
    public class Scp966Role : CustomRole
    {
        private readonly Dictionary<int, AhpStat.AhpProcess> ahpProcesses = new Dictionary<int, AhpStat.AhpProcess>();

        /// <inheritdoc />
        public override uint Id { get; set; } = 966;

        /// <inheritdoc />
        public override RoleType Role { get; set; } = RoleType.Scp0492;

        /// <inheritdoc />
        public override int MaxHealth { get; set; } = 600;

        /// <summary>
        /// Gets or sets the ahp to apply.
        /// </summary>
        public ConfiguredAhp HumeShield { get; set; } = new ConfiguredAhp(200f, 200f, -10f, 1f, 10f, true);

        /// <inheritdoc />
        public override string Name { get; set; } = "SCP-966";

        /// <inheritdoc />
        public override string Description { get; set; } = "An SCP that is invisible to all but a specific class of MTF.";

        /// <inheritdoc />
        public override string CustomInfo { get; set; } = "SCP-966";

        /// <summary>
        /// Gets or sets the percentage chance that the SCP will spawn in place of another scp in the spawn queue.
        /// </summary>
        [Description("The percentage chance that the SCP will spawn in place of another scp in the spawn queue.")]
        public Chance SpawnChance { get; set; } = new Chance(15f);

        /// <inheritdoc />
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties()
        {
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 100f,
                    Location = SpawnLocation.InsideServersBottom,
                },
            },
        };

        /// <inheritdoc />
        public override Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets a value indicating whether other Scps can see players with this role.
        /// </summary>
        [Description("Whether other Scps can see players with this role.")]
        public bool ScpsCanSee { get; set; } = true;

        /// <summary>
        /// Gets or sets the hint to display to players that are hit by this role.
        /// </summary>
        [Description("The hint to display to players that are hit by this role.")]
        public Hint HitHint { get; set; } = new Hint("You are being attacked by <color=red>Scp966</color>!", 5f);

        /// <inheritdoc />
        [YamlIgnore]
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        /// <inheritdoc />
        [YamlIgnore]
        public override bool RemovalKillsPlayer { get; set; } = true;

        /// <inheritdoc />
        [YamlIgnore]
        public override bool KeepRoleOnDeath { get; set; } = false;

        /// <inheritdoc />
        [YamlIgnore]
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>();

        /// <inheritdoc />
        [YamlIgnore]
        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new Dictionary<AmmoType, ushort>();

        /// <inheritdoc />
        [YamlIgnore]
        public override List<string> Inventory { get; set; } = new List<string>();

        /// <inheritdoc />
        public override void AddRole(Player player) => Timing.RunCoroutine(RunAddRole(player, player.Role != Role));

        /// <inheritdoc />
        protected override void SubscribeEvents()
        {
            PlayerStats.OnAnyPlayerDamaged += OnAnyPlayerDamaged;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            base.SubscribeEvents();
        }

        /// <inheritdoc />
        protected override void UnsubscribeEvents()
        {
            PlayerStats.OnAnyPlayerDamaged -= OnAnyPlayerDamaged;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            base.UnsubscribeEvents();
        }

        /// <inheritdoc />
        protected override void RoleAdded(Player player)
        {
            Log.Debug($"[{nameof(RoleAdded)}] {player}", Plugin.Instance.Config.Debug);
            if (!ahpProcesses.ContainsKey(player.Id))
                ahpProcesses.Add(player.Id, HumeShield.AddTo(player));

            foreach (Player ply in Player.List)
            {
                if (ply.IsDead || ply.SessionVariables.ContainsKey("Has966Goggles") || (ScpsCanSee && ply.IsScp))
                    continue;

                ply.TargetGhostsHashSet.Add(player.Id);
                Log.Debug($"[{nameof(RoleAdded)}] Added targeted invisibility, {ply} can no longer see {player}", Plugin.Instance.Config.Debug);
            }

            base.RoleAdded(player);
        }

        /// <inheritdoc />
        protected override void RoleRemoved(Player player)
        {
            Log.Debug($"[{nameof(RoleRemoved)}] {player}", Plugin.Instance.Config.Debug);
            if (ahpProcesses.TryGetValue(player.Id, out AhpStat.AhpProcess ahpProcess))
            {
                player.GetModule<AhpStat>().ServerKillProcess(ahpProcess.KillCode);
                ahpProcesses.Remove(player.Id);
            }

            foreach (Player ply in Player.List)
            {
                ply.TargetGhostsHashSet.Remove(player.Id);
                Log.Debug($"[{nameof(RoleRemoved)}] Removed targeted invisibility, {ply} can now see {player}", Plugin.Instance.Config.Debug);
            }

            base.RoleRemoved(player);
        }

        private void OnAnyPlayerDamaged(ReferenceHub referenceHub, DamageHandlerBase handler)
        {
            Player player = Player.Get(referenceHub);
            if (ahpProcesses.TryGetValue(player.Id, out AhpStat.AhpProcess ahpProcess))
                ahpProcess.SustainTime = HumeShield.SustainTime;
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Target == null)
                return;

            if (Check(ev.Attacker))
                HitHint.Display(ev.Target);
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            if (!ev.Player.SessionVariables.ContainsKey("SpawnScp966"))
                return;

            Log.Debug($"[{nameof(OnSpawned)}] Player has been selected to spawn as {nameof(Name)}, spawning {nameof(Name)}", Plugin.Instance.Config.Debug);
            AddRole(ev.Player);
            ev.Player.SessionVariables.Remove("SpawnScp966");
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            foreach (Player player in TrackedPlayers)
            {
                ev.Player.TargetGhostsHashSet.Add(player.Id);
                Log.Debug($"[{nameof(OnVerified)}] Added targeted invisibility, {ev.Player} can no longer see {player}", Plugin.Instance.Config.Debug);
            }
        }

        private IEnumerator<float> RunAddRole(Player player, bool isDelayed)
        {
            if (isDelayed)
            {
                player.SetRole(Role);
                yield return Timing.WaitForSeconds(1.5f);
            }

            Vector3 spawnPosition = GetSpawnPosition();
            if (spawnPosition != Vector3.zero)
                player.Position = spawnPosition + (Vector3.up * 1.5f);

            if (!KeepInventoryOnSpawn)
                player.ClearInventory();

            foreach (string itemName in Inventory)
                TryAddItem(player, itemName);

            player.Ammo.AddRange(Ammo);
            player.Health = MaxHealth;
            player.MaxHealth = MaxHealth;
            player.Scale = Scale;

            player.CustomInfo = CustomInfo;
            player.InfoArea &= ~PlayerInfoArea.Role;
            if (CustomAbilities != null)
            {
                foreach (CustomAbility customAbility in CustomAbilities)
                    customAbility.AddAbility(player);
            }

            ShowMessage(player);
            RoleAdded(player);
            TrackedPlayers.Add(player);
        }
    }
}