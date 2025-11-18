using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(CompApparelVerbOwner_Charged), nameof(CompApparelVerbOwner_Charged.PostPostMake))]
internal class CompApparelVerbOwner_Charged_PostPostMake
{
    [HarmonyPriority(0)]
    private static void Postfix(CompApparelVerbOwner_Charged __instance, ref int ___remainingCharges)
    {
        if (!YayoCombatCore.ammo || !__instance.parent.def.IsWeapon)
        {
            return;
        }

        ___remainingCharges = GenTicks.TicksGame <= 5
            ? Mathf.RoundToInt(__instance.Props.maxCharges * YayoCombatCore.s_enemyAmmo)
            : 0;
    }
}