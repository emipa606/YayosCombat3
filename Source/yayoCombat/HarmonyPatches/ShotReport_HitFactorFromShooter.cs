using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(ShotReport), nameof(ShotReport.HitFactorFromShooter))]
public static class ShotReport_HitFactorFromShooter
{
    public static void Postfix(Thing caster, float distance, float? acc, ref float __result)
    {
        if (!YayoCombatCore.advShootAcc || acc.HasValue || caster == null)
        {
            return;
        }

        if (caster is not Pawn pawn)
        {
            return;
        }

        if (!YayoCombatCore.mechAcc && pawn.RaceProps.IsMechanoid)
        {
            return;
        }

        if (YayoCombatCore.colonistAcc && !pawn.IsColonist)
        {
            return;
        }

        var baseAccuracy = pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn);
        var skillFactor = pawn.skills == null
            ? YayoCombatCore.baseSkill / 20f
            : pawn.skills.GetSkill(SkillDefOf.Shooting).levelInt / 20f;

        var skillBoostedAccuracy = UnityEngine.Mathf.Clamp01(baseAccuracy + (skillFactor * (1f - baseAccuracy)));
        var blendedAccuracy = UnityEngine.Mathf.Lerp(baseAccuracy, skillBoostedAccuracy, YayoCombatCore.s_accEf);

        __result = ShotReport.HitFactorFromShooter(caster, distance, blendedAccuracy);
    }
}
