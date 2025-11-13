using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Pawn), "Tick")]
internal class Pawn_Tick
{
    [HarmonyPriority(0)]
    private static void Postfix(Pawn __instance)
    {
        if (!YayoCombatCore.ammo || !__instance.Drafted || !__instance.IsHashIntervalTick(60) ||
            __instance.CurJobDef != JobDefOf.Wait_Combat && __instance.CurJobDef != JobDefOf.AttackStatic &&
            __instance.CurJobDef != JobDefOf.Reload ||
            __instance.equipment == null)
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