using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(DrugPolicy), "InitializeIfNeeded")]
public static class DrugPolicy_InitializeIfNeeded
{
    private static readonly AccessTools.FieldRef<DrugPolicy, List<DrugPolicyEntry>> s_entriesInt =
        AccessTools.FieldRefAccess<DrugPolicy, List<DrugPolicyEntry>>("entriesInt");

    [HarmonyPriority(0)]
    public static bool Prefix(DrugPolicy __instance)
    {
        if (!YayoCombatCore.ammo)
        {
            return true;
        }

        if (s_entriesInt(__instance) != null)
        {
            return false;
        }

        s_entriesInt(__instance) = [];
        var allDefsListForReading = DefDatabase<ThingDef>.AllDefsListForReading;
        var list = new List<DrugPolicyEntry>();
        foreach (var item in allDefsListForReading)
        {
            if (item.category == ThingCategory.Item &&
                (item.FirstThingCategory == ThingCategoryDef.Named("yy_ammo_category") ||
                 YayoCombatCore.ar_customAmmoDef.Contains(item)))
            {
                list.Add(new DrugPolicyEntry
                {
                    drug = item,
                    allowedForAddiction = false
                });
            }
        }

        list.SortBy(e =>
            (int)e.drug.techLevel +
            (e.drug.defName.Contains("fire") ? 0.1f : e.drug.defName.Contains("emp") ? 0.2f : 0f));
        var list2 = new List<DrugPolicyEntry>();
        foreach (var item2 in allDefsListForReading)
        {
            if (item2.category == ThingCategory.Item && item2.FirstThingCategory == ThingCategoryDefOf.Medicine)
            {
                list2.Add(new DrugPolicyEntry
                {
                    drug = item2,
                    allowedForAddiction = false
                });
            }
        }

        list2.SortByDescending(e => e.drug.BaseMarketValue);
        var list3 = new List<DrugPolicyEntry>();
        foreach (var item3 in allDefsListForReading)
        {
            if (item3.category == ThingCategory.Item && item3.IsDrug)
            {
                list3.Add(new DrugPolicyEntry
                {
                    drug = item3,
                    allowedForAddiction = true
                });
            }
        }

        list3.SortBy(e =>
            e.drug.GetCompProperties<CompProperties_Drug>() != null
                ? e.drug.GetCompProperties<CompProperties_Drug>().listOrder
                : 0f);
        var list4 = new List<DrugPolicyEntry>();
        foreach (var item4 in allDefsListForReading)
        {
            if (item4.category == ThingCategory.Item && item4.FirstThingCategory == ThingCategoryDefOf.Foods)
            {
                list4.Add(new DrugPolicyEntry
                {
                    drug = item4,
                    allowedForAddiction = false
                });
            }
        }

        list4.SortByDescending(e => e.drug.BaseMarketValue);
        s_entriesInt(__instance).AddRange(list);
        s_entriesInt(__instance).AddRange(list2);
        s_entriesInt(__instance).AddRange(list3);
        s_entriesInt(__instance).AddRange(list4);
        return false;
    }
}