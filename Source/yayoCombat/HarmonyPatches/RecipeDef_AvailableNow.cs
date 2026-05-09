using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(RecipeDef), nameof(RecipeDef.AvailableNow), MethodType.Getter)]
public static class RecipeDef_AvailableNow
{
    public static void Postfix(RecipeDef __instance, ref bool __result)
    {
        // If ammo system is disabled and this is an ammo recipe, it's not available
        if (!YayoCombatCore.ammo && __instance.defName.Contains("yy_ammo"))
        {
            __result = false;
        }
    }
}
