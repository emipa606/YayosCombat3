using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Pawn), "Tick")]
public static class Pawn_Tick
{
    [HarmonyPriority(0)]
    public static void Postfix(Pawn __instance)
    {
        if (!YayoCombatCore.ammo || !__instance.Spawned || __instance.Dead || __instance.Downed ||
            !__instance.IsHashIntervalTick(60) || __instance.equipment == null ||
            __instance.CurJobDef == JobDefOf.Reload)
        {
            return;
        }

        if (!__instance.Drafted && (__instance.Faction == null || __instance.Faction.IsPlayer))
        {
            return;
        }

        var isCombatJob = __instance.CurJobDef == JobDefOf.Wait_Combat ||
                          __instance.CurJobDef == JobDefOf.AttackStatic ||
                          !__instance.Drafted && __instance.CurJobDef == JobDefOf.AttackMelee;
        if (!isCombatJob)
        {
            return;
        }

        var allEquipmentListForReading = __instance.equipment.AllEquipmentListForReading;
        foreach (var item in allEquipmentListForReading)
        {
            var compApparelReloadable = item.TryGetComp<CompApparelReloadable>();
            if (compApparelReloadable == null)
            {
                continue;
            }

            reloadUtility.TryAutoReload(compApparelReloadable);
            break;
        }
    }
}