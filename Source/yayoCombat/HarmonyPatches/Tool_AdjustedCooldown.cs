using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Tool), nameof(Tool.AdjustedCooldown), typeof(Thing))]
public static class Tool_AdjustedCooldown
{
    [HarmonyPriority(0)]
    public static void Postfix(ref float __result, Thing ownerEquipment)
    {
        if (YayoCombatCore.meleeDelay == 1f && YayoCombatCore.meleeRandom <= 0f)
        {
            return;
        }

        if (ownerEquipment == null)
        {
            return;
        }

        if (!(__result > 0f))
        {
            return;
        }

        if (ownerEquipment.def is not { IsMeleeWeapon: true })
        {
            return;
        }

        var randomFactor = 1f + ((Rand.Value - 0.5f) * YayoCombatCore.meleeRandom);
        var multiplier = Mathf.Max(0.01f, YayoCombatCore.meleeDelay * randomFactor);
        __result = Mathf.Max(__result * multiplier, 0.2f);
    }
}