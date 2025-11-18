using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat;

public static class ArmorUtility
{
    public const float MaxArmorRating = 2f;

    private const float DeflectThresholdFactor = 0.5f;

    public static float GetPostArmorDamage(Pawn pawn, float amount, float armorPenetration, BodyPartRecord part,
        ref DamageDef damageDef, ref bool deflectedByMetalArmor, ref bool diminishedByMetalArmor, DamageInfo dinfo)
    {
        deflectedByMetalArmor = false;
        diminishedByMetalArmor = false;
        bool forcedDefl;
        if (damageDef.armorCategory == null)
        {
            return amount;
        }

        var armorRatingStat = damageDef.armorCategory.armorRatingStat;
        if (pawn.apparel != null)
        {
            var wornApparel = pawn.apparel.WornApparel;
            for (var num = wornApparel.Count - 1; num >= 0; num--)
            {
                var apparel = wornApparel[num];
                if (!apparel.def.apparel.CoversBodyPart(part))
                {
                    continue;
                }

                var num2 = amount;
                applyArmor(ref amount, armorPenetration, apparel.GetStatValue(armorRatingStat), apparel,
                    ref damageDef, pawn, out var metalArmor, dinfo, out forcedDefl);
                if (amount < 0.001f)
                {
                    deflectedByMetalArmor = metalArmor || forcedDefl;

                    return 0f;
                }

                if (amount < num2 && metalArmor)
                {
                    diminishedByMetalArmor = true;
                }
            }
        }

        var num3 = amount;
        applyArmor(ref amount, armorPenetration, pawn.GetStatValue(armorRatingStat), null, ref damageDef, pawn,
            out var metalArmor2, dinfo, out forcedDefl);
        if (amount < 0.001f)
        {
            deflectedByMetalArmor = metalArmor2 || forcedDefl;

            return 0f;
        }

        if (amount < num3 && metalArmor2)
        {
            diminishedByMetalArmor = true;
        }

        if (forcedDefl)
        {
            deflectedByMetalArmor = true;
        }

        return amount;
    }

    private static void applyArmor(ref float damAmount, float armorPenetration, float armorRating, Thing armorThing,
        ref DamageDef damageDef, Pawn pawn, out bool metalArmor, DamageInfo dinfo, out bool forcedDefl)
    {
        var isArmor = false;
        var isMechanoid = pawn.RaceProps.IsMechanoid;
        forcedDefl = false;
        if (armorThing != null)
        {
            isArmor = true;
            metalArmor = armorThing.def.apparel.useDeflectMetalEffect ||
                         armorThing.Stuff is { IsMetal: true };
        }
        else
        {
            metalArmor = isMechanoid;
        }

        var num = armorPenetration;
        if (isArmor && dinfo.Weapon != null)
        {
            if (armorThing.def.techLevel >= TechLevel.Spacer || isMechanoid)
            {
                if (dinfo.Weapon.IsMeleeWeapon)
                {
                    if (dinfo.Weapon.techLevel <= TechLevel.Medieval)
                    {
                        num *= 0.5f;
                    }
                }
                else if (dinfo.Weapon.techLevel <= TechLevel.Medieval)
                {
                    num *= 0.35f;
                }
            }
            else if (armorThing.def.techLevel >= TechLevel.Industrial)
            {
                if (dinfo.Weapon.IsMeleeWeapon)
                {
                    if (dinfo.Weapon.techLevel <= TechLevel.Neolithic)
                    {
                        num *= 0.75f;
                    }
                }
                else if (dinfo.Weapon.techLevel <= TechLevel.Medieval)
                {
                    num *= 0.5f;
                }
            }
        }

        var leftArmor = Mathf.Max(armorRating - num, 0f);
        var armorDmg = (num - (armorRating * 0.15f)) * 5f;
        armorDmg = Mathf.Clamp01(armorDmg);
        var randomZeroOne = Rand.Value;
        if (isArmor)
        {
            var f = damAmount * (0.2f + (armorDmg * DeflectThresholdFactor));
            armorThing.TakeDamage(new DamageInfo(damageDef, GenMath.RoundRandom(f)));
        }

        var armorHpPer = !isArmor
            ? pawn.health.summaryHealth.SummaryHealthPercent
            : armorThing.HitPoints / (float)armorThing.MaxHitPoints;
        var defenceRating = Mathf.Max((armorRating * 0.9f) - num, 0f);
        var getHitRating = 1f - YayoCombatCore.s_armorEf;
        if (randomZeroOne * getHitRating < defenceRating * armorHpPer)
        {
            if (Rand.Value < Mathf.Min(leftArmor, 0.9f))
            {
                damAmount = 0f;
            }
            else if (isArmor)
            {
                forcedDefl = true;
                damAmount = GenMath.RoundRandom(damAmount * (0.25f + (armorDmg * 0.25f)));
                if (damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp)
                {
                    damageDef = DamageDefOf.Blunt;
                }
            }
            else
            {
                forcedDefl = true;
                damAmount = GenMath.RoundRandom(damAmount * (0.25f + (armorDmg * DeflectThresholdFactor)));
                if (damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp)
                {
                    damageDef = DamageDefOf.Blunt;
                }
            }
        }
        else if (randomZeroOne < leftArmor * (0.5f + (armorHpPer * DeflectThresholdFactor)))
        {
            damAmount = GenMath.RoundRandom(damAmount * DeflectThresholdFactor);
            if (damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp)
            {
                damageDef = DamageDefOf.Blunt;
            }
        }
    }
}