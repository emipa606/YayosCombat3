using HarmonyLib;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.DropAllEquipment))]
public static class Pawn_EquipmentTracker_DropAllEquipment
{
    public static void Prefix(Pawn_EquipmentTracker __instance, ThingOwner<ThingWithComps> ___equipment)
    {
        if (!YayoCombatCore.ammo || __instance.pawn.Faction?.IsPlayer == true)
        {
            return;
        }

        foreach (var thing in ___equipment)
        {
            reloadUtility.TryThingEjectAmmoDirect(thing, true, __instance.pawn);
        }
    }
}