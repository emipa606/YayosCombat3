using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(TraderKindDef), nameof(TraderKindDef.WillTrade))]
public static class TraderKindDef_WillTrade
{
    public static bool Prefix(ref bool __result, TraderKindDef __instance, ThingDef td)
    {
        if(!YayoCombatCore.ammo)
        {
            return true;
        }

        if(__instance.defName == "Empire_Caravan_TributeCollector")
        {
            return true;
        }

        if(td.tradeTags == null || !td.tradeTags.Contains("Ammo"))
        {
            return true;
        }

        __result = true;
        return false;
    }
}