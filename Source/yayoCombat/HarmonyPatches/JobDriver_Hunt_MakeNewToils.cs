using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(JobDriver_Hunt), "MakeNewToils")]
public static class JobDriver_Hunt_MakeNewToils
{
    public static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, Pawn ___pawn)
    {
        foreach (var toil in values)
        {
            toil.FailOn(() => ShouldFailForNoAmmo(___pawn));
            yield return toil;
        }
    }

    private static bool ShouldFailForNoAmmo(Pawn pawn)
    {
        if (!YayoCombatCore.ammo)
        {
            return false;
        }

        if (pawn?.CurJobDef != JobDefOf.Hunt)
        {
            return false;
        }

        var primary = pawn.equipment?.Primary;
        if (primary == null || !primary.def.IsRangedWeapon)
        {
            return false;
        }

        var reloadable = primary.TryGetComp<CompApparelReloadable>();
        if (reloadable == null)
        {
            return false;
        }

        return !reloadable.CanBeUsed(out _);
    }
}
