using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
public static class Thing_TakeDamage
{
    private static void Prefix(ref DamageInfo dinfo)
    {
        if (YayoCombatCore.meleeDelay != 1)
        {
            return;
        }

        if (!(dinfo.Amount > 0f))
        {
            return;
        }

        if (dinfo.Weapon is not { IsMeleeWeapon: true })
        {
            return;
        }

        var amount = Mathf.Max(1f, Mathf.RoundToInt(dinfo.Amount * YayoCombatCore.meleeDelay));
        dinfo.SetAmount(amount);
    }
}