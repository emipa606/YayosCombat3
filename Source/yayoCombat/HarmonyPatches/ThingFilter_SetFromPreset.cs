using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(ThingFilter), nameof(ThingFilter.SetFromPreset))]
internal class ThingFilter_SetFromPreset
{
    private static bool Prefix(ThingFilter __instance, StorageSettingsPreset preset)
    {
        if (!YayoCombatCore.ammo)
        {
            return true;
        }

        if (preset == StorageSettingsPreset.DefaultStockpile)
        {
            __instance.SetAllow(ThingCategoryDef.Named("yy_ammo_category"), true);
        }

        return true;
    }
}