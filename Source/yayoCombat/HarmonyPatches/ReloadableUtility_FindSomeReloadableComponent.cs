using HarmonyLib;
using RimWorld;
using RimWorld.Utility;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(ReloadableUtility), nameof(ReloadableUtility.FindSomeReloadableComponent))]
internal class ReloadableUtility_FindSomeReloadableComponent
{
    private static void Postfix(ref IReloadableComp __result, Pawn pawn, bool allowForcedReload)
    {
        if (!yayoCombat.ammo || __result != null)
        {
            return;
        }

        if (pawn?.equipment == null)
        {
            return;
        }

        foreach (var thing in pawn.equipment.AllEquipmentListForReading)
        {
            var compApparelReloadable = thing.TryGetComp<CompApparelReloadable>();
            if (compApparelReloadable?.NeedsReload(allowForcedReload) != true)
            {
                continue;
            }

            __result = compApparelReloadable;
            return;
        }
    }
}