using HarmonyLib;
using RimWorld;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(CompApparelReloadable), "UsedOnce")]
public static class CompApparelReloadable_UsedOnce
{
    // This patch ensures the base UsedOnce behavior works correctly with ammo system
    // The vanilla implementation already handles charge depletion
}