using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Verse.DamageWorker_AddInjury), "ApplyDamageToPart", typeof(DamageInfo), typeof(Pawn),
    typeof(DamageWorker.DamageResult))]
internal class DamageWorker_AddInjury
{
    private static bool Prefix(Verse.DamageWorker_AddInjury __instance, DamageInfo dinfo, Pawn pawn,
        DamageWorker.DamageResult result)
    {
        if (!YayoCombatCore.advArmor)
        {
            return true;
        }

        var exactPartFromDamageInfo = getExactPartFromDamageInfo(dinfo, pawn);
        if (exactPartFromDamageInfo == null)
        {
            return false;
        }

        dinfo.SetHitPart(exactPartFromDamageInfo);
        var num = dinfo.Amount;
        var deflectedByMetalArmor = false;
        if (dinfo is { InstantPermanentInjury: false, IgnoreArmor: false })
        {
            var damageDef = dinfo.Def;
            var diminishedByMetalArmor = false;
            num = ArmorUtility.GetPostArmorDamage(pawn, num, dinfo.ArmorPenetrationInt, dinfo.HitPart, ref damageDef,
                ref deflectedByMetalArmor, ref diminishedByMetalArmor, dinfo);
            dinfo.Def = damageDef;
            if (num < dinfo.Amount)
            {
                result.diminished = true;
                result.diminishedByMetalArmor = diminishedByMetalArmor;
            }
        }

        if (dinfo.Def.ExternalViolenceFor(pawn))
        {
            num *= pawn.GetStatValue(StatDefOf.IncomingDamageFactor);
        }

        if (deflectedByMetalArmor)
        {
            result.deflected = true;
            result.deflectedByMetalArmor = true;
        }

        if (num <= 0f)
        {
            result.AddPart(pawn, dinfo.HitPart);
            return false;
        }

        if (isHeadshot(dinfo))
        {
            result.headshot = true;
        }

        if (dinfo.InstantPermanentInjury &&
            (HealthUtility.GetHediffDefFromDamage(dinfo.Def, pawn, dinfo.HitPart)
                 .CompPropsFor(typeof(HediffComp_GetsPermanent)) == null ||
             dinfo.HitPart.def.permanentInjuryChanceFactor == 0f ||
             pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(dinfo.HitPart)))
        {
            return false;
        }

        if (!dinfo.AllowDamagePropagation)
        {
            finalizeAndAddInjury(pawn, num, dinfo, result);
            return false;
        }

        var methodInfo = typeof(Verse.DamageWorker_AddInjury).GetMethod("ApplySpecialEffectsToPart",
            BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo?.Invoke(__instance, [pawn, num, dinfo, result]);
        //__instance.ApplySpecialEffectsToPart(pawn, num, dinfo, result);
        return false;
    }

    private static BodyPartRecord getExactPartFromDamageInfo(DamageInfo damageInfo, Pawn pawn)
    {
        if (damageInfo.HitPart != null)
        {
            return pawn.health.hediffSet.GetNotMissingParts().All(x => x != damageInfo.HitPart)
                ? null
                : damageInfo.HitPart;
        }

        var bodyPartRecord = chooseHitPart(damageInfo, pawn);
        if (bodyPartRecord == null)
        {
            Log.Warning("ChooseHitPart returned null (any part).");
        }

        return bodyPartRecord;
    }

    private static BodyPartRecord chooseHitPart(DamageInfo damageInfo, Pawn pawn)
    {
        return pawn.health.hediffSet.GetRandomNotMissingPart(damageInfo.Def, damageInfo.Height, damageInfo.Depth);
    }

    private static bool isHeadshot(DamageInfo damageInfo)
    {
        return !damageInfo.InstantPermanentInjury && damageInfo.HitPart.groups.Contains(BodyPartGroupDefOf.FullHead) &&
               damageInfo.Def == DamageDefOf.Bullet;
    }

    private static void finalizeAndAddInjury(Pawn pawn, float totalDamage, DamageInfo damageInfo,
        DamageWorker.DamageResult result)
    {
        if (pawn.health.hediffSet.PartIsMissing(damageInfo.HitPart))
        {
            return;
        }

        var hediffDefFromDamage = HealthUtility.GetHediffDefFromDamage(damageInfo.Def, pawn, damageInfo.HitPart);
        var hediffInjury = (Hediff_Injury)HediffMaker.MakeHediff(hediffDefFromDamage, pawn);
        hediffInjury.Part = damageInfo.HitPart;
        hediffInjury.sourceDef = damageInfo.Weapon;
        hediffInjury.sourceBodyPartGroup = damageInfo.WeaponBodyPartGroup;
        hediffInjury.sourceHediffDef = damageInfo.WeaponLinkedHediff;
        hediffInjury.Severity = totalDamage;
        if (damageInfo.InstantPermanentInjury)
        {
            var hediffComp_GetsPermanent = hediffInjury.TryGetComp<HediffComp_GetsPermanent>();
            if (hediffComp_GetsPermanent != null)
            {
                hediffComp_GetsPermanent.IsPermanent = true;
            }
            else
            {
                Log.Error(
                    $"Tried to create instant permanent injury on Hediff without a GetsPermanent comp: {hediffDefFromDamage} on {pawn}");
            }
        }

        finalizeAndAddInjury(pawn, hediffInjury, damageInfo, result);
    }

    private static void finalizeAndAddInjury(Pawn pawn, Hediff_Injury injury, DamageInfo damageInfo,
        DamageWorker.DamageResult result)
    {
        injury.TryGetComp<HediffComp_GetsPermanent>()?.PreFinalizeInjury();
        var partHealth = pawn.health.hediffSet.GetPartHealth(injury.Part);
        if (pawn.IsColonist && !damageInfo.IgnoreInstantKillProtection && damageInfo.Def.ExternalViolenceFor(pawn) &&
            !Rand.Chance(Find.Storyteller.difficulty.allowInstantKillChance))
        {
            var num = injury.def.lethalSeverity > 0f ? injury.def.lethalSeverity * 1.1f : 1f;
            var min = 1f;
            var max = Mathf.Min(injury.Severity, partHealth);
            for (var i = 0; i < 7; i++)
            {
                if (!pawn.health.WouldDieAfterAddingHediff(injury))
                {
                    break;
                }

                var num2 = Mathf.Clamp(partHealth - num, min, max);
                if (DebugViewSettings.logCauseOfDeath)
                {
                    Log.Message(
                        $"CauseOfDeath: attempt to prevent death for {pawn.Name} on {injury.Part.Label} attempt:{i + 1} severity:{injury.Severity}->{num2} part health:{partHealth}");
                }

                injury.Severity = num2;
                num *= 2f;
                min = 0f;
            }
        }

        pawn.health.AddHediff(injury, null, damageInfo, result);
        var num3 = Mathf.Min(injury.Severity, partHealth);
        result.totalDamageDealt += num3;
        result.wounded = true;
        result.AddPart(pawn, injury.Part);
        result.AddHediff(injury);
    }
}