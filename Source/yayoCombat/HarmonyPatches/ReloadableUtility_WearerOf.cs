using HarmonyLib;
using RimWorld;
using RimWorld.Utility;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(ReloadableUtility), nameof(ReloadableUtility.OwnerOf))]
internal class ReloadableUtility_WearerOf
{
    private static void Postfix(ref Pawn __result, IReloadableComp reloadable)
    {
        if (!YayoCombatCore.ammo || __result != null)
        {
            return;
        }

        if (reloadable is CompApparelReloadable
            {
                ParentHolder: Pawn_EquipmentTracker { pawn: not null } equipmentTracker
            })
        {
            __result = equipmentTracker.pawn;
        }
    }
}