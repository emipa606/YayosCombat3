using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.GetGizmos))]
internal class ThingWithComps_GetGizmos
{
    private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn_EquipmentTracker __instance)
    {
        foreach (var gizmo in __result)
        {
            yield return gizmo;
        }

        if (!YayoCombatCore.ammo || !PawnAttackGizmoUtility.CanShowEquipmentGizmos())
        {
            yield break;
        }

        foreach (var thingWithComps in __instance.AllEquipmentListForReading)
        {
            foreach (var comp in thingWithComps.AllComps)
            {
                foreach (var gizmo in comp.CompGetWornGizmosExtra())
                {
                    yield return gizmo;
                }
            }
        }
    }
}