using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(JobGiver_MoveDrugsToInventory), "FindDrugFor")]
public static class JobGiver_MoveDrugsToInventory_FindDrugFor
{
    public static bool Prefix(ref Thing __result, Pawn pawn,
        ThingDef drugDef)
    {
        if (!YayoCombatCore.ammo)
        {
            return true;
        }

        if (drugDef.IsDrug)
        {
            __result = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(drugDef),
                PathEndMode.ClosestTouch, TraverseParms.For(pawn));
        }
        else
        {
            __result = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(drugDef),
                PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f,
                x => !x.IsForbidden(pawn) && pawn.CanReserve(x));
        }

        return false;
    }
}