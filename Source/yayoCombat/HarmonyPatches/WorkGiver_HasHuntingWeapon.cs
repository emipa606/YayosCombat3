using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(WorkGiver_HunterHunt), nameof(WorkGiver_HunterHunt.HasHuntingWeapon))]
public static class WorkGiver_HasHuntingWeapon
{
    public static bool Postfix(bool __result, Pawn p)
    {
        if(!YayoCombatCore.ammo || !__result)
        {
            return __result;
        }

        var comp = p.equipment.Primary.GetComp<CompApparelReloadable>();
        if(comp != null)
        {
            __result = comp.CanBeUsed(out _);
        }

        return __result;
    }
}