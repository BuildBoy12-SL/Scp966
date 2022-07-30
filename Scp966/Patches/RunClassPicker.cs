// -----------------------------------------------------------------------
// <copyright file="RunClassPicker.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Scp966.Patches
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Features;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using Scp966.CustomRoles;
    using Scp966.Models;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.RunDefaultClassPicker"/> to have a chance for <see cref="Scp966Role"/> to replace an Scp spawn.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RunDefaultClassPicker))]
    internal static class RunClassPicker
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder scp966Role = generator.DeclareLocal(typeof(Scp966Role));
            LocalBuilder selected966 = generator.DeclareLocal(typeof(bool));

            Label skipReplaceLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_0);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, selected966.LocalIndex),
            });

            int offset = 2;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Add) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, selected966.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Brtrue_S, skipReplaceLabel),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager.Classes))),
                new CodeInstruction(OpCodes.Ldloc_S, 10),
                new CodeInstruction(OpCodes.Call, Method(typeof(RoleExtensionMethods), nameof(RoleExtensionMethods.SafeGet), new[] { typeof(Role[]), typeof(RoleType) })),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.team))),
                new CodeInstruction(OpCodes.Brtrue_S, skipReplaceLabel),

                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Plugin), nameof(Plugin.Instance))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Plugin), nameof(Plugin.Config))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.Scp966))),
                new CodeInstruction(OpCodes.Stloc_S, scp966Role.LocalIndex),

                new CodeInstruction(OpCodes.Ldloc_S, scp966Role.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp966Role), nameof(Scp966Role.SpawnChance))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Chance), nameof(Chance.IsSuccess))),
                new CodeInstruction(OpCodes.Brfalse_S, skipReplaceLabel),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager.Classes))),
                new CodeInstruction(OpCodes.Ldloc_S, 10),
                new CodeInstruction(OpCodes.Call, Method(typeof(RoleExtensionMethods), nameof(RoleExtensionMethods.Get), new[] { typeof(Role[]), typeof(RoleType) })),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(Role), nameof(Role.banClass))),

                new CodeInstruction(OpCodes.Ldloc_S, scp966Role.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp966Role), nameof(Scp966Role.Role))),
                new CodeInstruction(OpCodes.Stloc_S, 10),

                new CodeInstruction(OpCodes.Ldloc_S, 9),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.SessionVariables))),
                new CodeInstruction(OpCodes.Ldstr, "SpawnScp966"),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Box, typeof(bool)),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<string, object>), nameof(Dictionary<string, object>.Add))),

                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_S, selected966.LocalIndex),
            });

            offset = -3;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_3) + offset;
            newInstructions[index].labels.Add(skipReplaceLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}