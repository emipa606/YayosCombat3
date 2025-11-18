using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_DrugPolicyTracker), nameof(Pawn_DrugPolicyTracker.AllowedToTakeToInventory))]
internal class Pawn_DrugPolicyTracker_AllowedToTakeToInventory
{
    private static bool Prefix(ref bool __result, Pawn_DrugPolicyTracker __instance, ThingDef thingDef)
    {
        if (!YayoCombatCore.ammo)
        {
            return true;
        }

        if (thingDef.FirstThingCategory == ThingCategoryDef.Named("yy_ammo_category") ||
            YayoCombatCore.ar_customAmmoDef.Contains(thingDef) ||
            thingDef.FirstThingCategory == ThingCategoryDefOf.Medicine ||
            thingDef.FirstThingCategory == ThingCategoryDefOf.Foods)
        {
            var drugPolicyEntry = __instance.CurrentPolicy[thingDef];
            __result = !drugPolicyEntry.allowScheduled && drugPolicyEntry.takeToInventory > 0 &&
                       drugPolicyEntry.takeToInventory >
                       __instance.pawn.inventory.innerContainer.TotalStackCountOfDef(thingDef);
            return false;
        }

        if (!thingDef.IsIngestible)
        {
            Log.Error($"{thingDef} is not ingestible.");
            __result = false;
            return false;
        }

        if (!thingDef.IsDrug)
        {
            Log.Error($"AllowedToTakeScheduledEver on non-drug {thingDef}");
            __result = false;
            return false;
        }

        if (thingDef.IsNonMedicalDrug && __instance.pawn.IsTeetotaler())
        {
            __result = false;
            return false;
        }

        var drugPolicyEntry2 = __instance.CurrentPolicy[thingDef];
        __result = !drugPolicyEntry2.allowScheduled && drugPolicyEntry2.takeToInventory > 0 &&
                   !__instance.pawn.inventory.innerContainer.Contains(thingDef);
        return false;
    }
}