using HarmonyLib;
using RimWorld;
using RimWorld.Utility;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(ReloadableUtility), nameof(ReloadableUtility.OwnerOf))]
public static class ReloadableUtility_WearerOf
{
    public static void Postfix(ref Pawn __result, IReloadableComp reloadable)
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