using HarmonyLib;
using RimWorld;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(CompApparelReloadable), nameof(CompApparelReloadable.CompTick))]
public static class CompApparelReloadable_CompTick
{
    public static void Prefix(CompApparelReloadable __instance, ref int ___replenishInTicks)
    {
        if (__instance.AmmoDef != null || !__instance.Props.replenishAfterCooldown || ___replenishInTicks != -1)
        {
            return;
        }

        ___replenishInTicks = __instance.Props.baseReloadTicks;
    }
}
