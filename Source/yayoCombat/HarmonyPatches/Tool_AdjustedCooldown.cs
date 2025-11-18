using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Tool), nameof(Tool.AdjustedCooldown), typeof(Thing))]
internal class Tool_AdjustedCooldown
{
    [HarmonyPriority(0)]
    public static void Postfix(ref float __result, Thing ownerEquipment)
    {
        if(YayoCombatCore.meleeRandom > 0)
        {
            return;
        }

        if(ownerEquipment == null)
        {
            return;
        }

        if(!(__result > 0f))
        {
            return;
        }

        if(ownerEquipment.ParentHolder is not { ParentHolder: Pawn })
        {
            return;
        }

        if(ownerEquipment.def is not { IsMeleeWeapon: true })
        {
            return;
        }

        var num = YayoCombatCore.meleeDelay * (1f + ((Rand.Value - 0.5f) * YayoCombatCore.meleeRandom));
        __result = Mathf.Max(__result * num, 0.2f);
    }
}