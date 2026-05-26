using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Verb_LaunchProjectile), "TryCastShot")]
public static class Verb_LaunchProjectile_TryCastShot
{
    private static bool UseAdvancedAccuracyForShooter(Thing caster, bool casterIsPawn, Pawn casterPawn)
    {
        if (!YayoCombatCore.advShootAcc)
        {
            return false;
        }

        if (!casterIsPawn)
        {
            return YayoCombatCore.turretAcc;
        }

        if (!YayoCombatCore.mechAcc && casterPawn.RaceProps.IsMechanoid)
        {
            return false;
        }

        return !YayoCombatCore.colonistAcc || casterPawn.IsColonist;
    }

    public static bool Prefix(
        ref bool __result,
        Verb_LaunchProjectile __instance,
        LocalTargetInfo ___currentTarget,
        bool ___canHitNonTargetPawnsNow,
        bool ___preventFriendlyFire)
    {
        var localTargetInfo = ___currentTarget;
        if (!UseAdvancedAccuracyForShooter(__instance.Caster, __instance.CasterIsPawn, __instance.CasterPawn))
        {
            return true;
        }

        if (localTargetInfo.HasThing && localTargetInfo.Thing.Map != __instance.caster.Map)
        {
            __result = false;
            return false;
        }

        var projectile = __instance.Projectile;
        if (projectile == null)
        {
            __result = false;
            return false;
        }

        var los_Successful =
            __instance.TryFindShootLineFromTo(__instance.caster.Position, localTargetInfo, out var resultingLine);
        if (__instance.verbProps.stopBurstWithoutLos && !los_Successful)
        {
            __result = false;
            return false;
        }

        if (__instance.EquipmentSource != null)
        {
            __instance.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
            __instance.EquipmentSource.GetComp<CompApparelReloadable>()?.UsedOnce();
        }

        var launcher = __instance.caster;
        Thing equipment = __instance.EquipmentSource;
        var compMannable = __instance.caster.TryGetComp<CompMannable>();
        if (compMannable is { ManningPawn: not null })
        {
            launcher = compMannable.ManningPawn;
            equipment = __instance.caster;
        }

        var drawPos = __instance.caster.DrawPos;
        var projectile2 = (Projectile)GenSpawn.Spawn(projectile, resultingLine.Source, __instance.caster.Map);
        if (equipment.TryGetComp(out CompUniqueWeapon comp))
        {
            foreach (var item in comp.TraitsListForReading)
            {
                if (item.damageDefOverride != null)
                {
                    projectile2.damageDefOverride = item.damageDefOverride;
                }

                if (item.extraDamages.NullOrEmpty())
                {
                    continue;
                }

                projectile2.extraDamages ??= [];

                projectile2.extraDamages.AddRange(item.extraDamages);
            }
        }

        if (__instance.verbProps.ForcedMissRadius > 0.5f)
        {
            var num = VerbUtility.CalculateAdjustedForcedMiss(
                __instance.verbProps.ForcedMissRadius,
                localTargetInfo.Cell - __instance.caster.Position);
            if (num > 0.5f)
            {
                var max = GenRadial.NumCellsInRadius(num);
                var num2 = Rand.Range(0, max);
                if (num2 > 0)
                {
                    var intVec = localTargetInfo.Cell + GenRadial.RadialPattern[num2];
                    var projectileHitTypes = ProjectileHitFlags.NonTargetWorld;
                    if (Rand.Chance(YayoCombatCore.s_missBulletHit))
                    {
                        projectileHitTypes = ProjectileHitFlags.All;
                    }

                    if (!___canHitNonTargetPawnsNow)
                    {
                        projectileHitTypes &= ~ProjectileHitFlags.NonTargetPawns;
                    }

                    projectile2.Launch(
                        launcher,
                        drawPos,
                        intVec,
                        localTargetInfo,
                        projectileHitTypes,
                        ___preventFriendlyFire,
                        equipment);
                    __result = true;
                    return false;
                }
            }
        }

        var shotReport = ShotReport.HitReportFor(__instance.caster, __instance, localTargetInfo);
        var randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
        var targetCoverDef = randomCoverToMissInto?.def;

        var hitChance = Mathf.Clamp(shotReport.AimOnTargetChance_IgnoringPosture, 0.0201f, 0.99f);
        if (!Rand.Chance(hitChance))
        {
            resultingLine.ChangeDestToMissWild(
                shotReport.AimOnTargetChance_StandardTarget,
                false,
                __instance.caster != null ? __instance.caster.Map : localTargetInfo.Thing.Map);
            var targetPawns = ProjectileHitFlags.NonTargetWorld;
            if (Rand.Chance(YayoCombatCore.s_missBulletHit) && ___canHitNonTargetPawnsNow)
            {
                targetPawns |= ProjectileHitFlags.NonTargetPawns;
            }

            projectile2.Launch(
                launcher,
                drawPos,
                resultingLine.Dest,
                localTargetInfo,
                targetPawns,
                ___preventFriendlyFire,
                equipment,
                targetCoverDef);
            __result = true;
            return false;
        }

        if (localTargetInfo.Thing != null &&
            localTargetInfo.Thing.def.category == ThingCategory.Pawn &&
            !Rand.Chance(shotReport.PassCoverChance))
        {
            var targetPawns = ProjectileHitFlags.NonTargetWorld;
            if (___canHitNonTargetPawnsNow)
            {
                targetPawns |= ProjectileHitFlags.NonTargetPawns;
            }

            projectile2.Launch(
                launcher,
                drawPos,
                randomCoverToMissInto,
                localTargetInfo,
                targetPawns,
                ___preventFriendlyFire,
                equipment,
                targetCoverDef);
            __result = true;
            return false;
        }

        var intendedTarget = ProjectileHitFlags.IntendedTarget;
        if (___canHitNonTargetPawnsNow)
        {
            intendedTarget |= ProjectileHitFlags.NonTargetPawns;
        }

        if (!localTargetInfo.HasThing || localTargetInfo.Thing.def.Fillage == FillCategory.Full)
        {
            intendedTarget |= ProjectileHitFlags.NonTargetWorld;
        }

        projectile2.Launch(
            launcher,
            drawPos,
            localTargetInfo.Thing != null ? localTargetInfo : resultingLine.Dest,
            localTargetInfo,
            intendedTarget,
            ___preventFriendlyFire,
            equipment,
            targetCoverDef);

        __result = true;
        return false;
    }
}