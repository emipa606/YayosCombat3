using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;
using ApparelLayerDefOf = yayoCombat_Defs.ApparelLayerDefOf;
using BodyPartGroupDefOf = yayoCombat_Defs.BodyPartGroupDefOf;

namespace yayoCombat;

public class yayoCombat : ModBase
{
    public static readonly bool using_dualWeld;

    public static readonly bool using_meleeAnimations;

    public static readonly bool using_showHands;

    public static readonly bool using_AlienRaces;

    private static readonly bool using_Oversized;

    private static readonly Dictionary<ThingDef, Vector3> southOffsets = new();
    private static readonly Dictionary<ThingDef, Vector3> northOffsets = new();
    private static readonly Dictionary<ThingDef, Vector3> eastOffsets = new();
    private static readonly Dictionary<ThingDef, Vector3> westOffsets = new();

    public static Dictionary<Thing, Tuple<Vector3, float>> weaponLocations;

    public static bool refillMechAmmo;

    public static bool ammo;

    public static float ammoGen;

    private static float maxAmmo;

    private static int enemyAmmo;

    public static float s_enemyAmmo;

    public static int supplyAmmoDist;

    public static float meleeDelay;

    public static float meleeRandom;

    private static bool handProtect;

    public static bool advArmor;

    private static int armorEf;

    public static float s_armorEf;

    public static float unprotectDmg;

    public static bool advShootAcc;

    private static int accEf;

    public static float s_accEf;

    private static int missBulletHit;

    public static float s_missBulletHit;

    public static bool mechAcc;

    public static bool turretAcc;

    public static int baseSkill;

    public static bool colonistAcc;

    public static float bulletSpeed;

    public static float maxBulletSpeed;

    private static bool enemyRocket;

    public static readonly List<ThingDef> ar_customAmmoDef;

    private SettingHandle<int> accEfSetting;

    private SettingHandle<bool> advArmorSetting;

    private SettingHandle<bool> advShootAccSetting;

    private SettingHandle<float> ammoGenSetting;

    private SettingHandle<bool> ammoSetting;

    private SettingHandle<int> armorEfSetting;

    private SettingHandle<int> baseSkillSetting;

    private SettingHandle<float> bulletSpeedSetting;

    private SettingHandle<bool> colonistAccSetting;

    private SettingHandle<int> enemyAmmoSetting;

    private SettingHandle<bool> enemyRocketSetting;

    private SettingHandle<bool> handProtectSetting;

    private SettingHandle<float> maxAmmoSetting;

    private SettingHandle<float> maxBulletSpeedSetting;

    private SettingHandle<bool> mechAccSetting;

    private SettingHandle<float> meleeDelaySetting;

    private SettingHandle<float> meleeRandomSetting;

    private SettingHandle<int> missBulletHitSetting;

    private SettingHandle<bool> refillMechAmmoSetting;

    private SettingHandle<int> supplyAmmoDistSetting;

    private SettingHandle<bool> turretAccSetting;

    private SettingHandle<float> unprotectDmgSetting;

    static yayoCombat()
    {
        using_dualWeld = false;
        using_meleeAnimations = false;
        using_AlienRaces = false;
        using_showHands = false;
        refillMechAmmo = true;
        ammo = false;
        ammoGen = 1f;
        maxAmmo = 1f;
        enemyAmmo = 70;
        s_enemyAmmo = 0.7f;
        supplyAmmoDist = 4;
        meleeDelay = 0.7f;
        meleeRandom = 1.3f;
        handProtect = true;
        advArmor = true;
        armorEf = 50;
        s_armorEf = 0.5f;
        unprotectDmg = 1.1f;
        advShootAcc = true;
        accEf = 60;
        s_accEf = 0.6f;
        missBulletHit = 50;
        s_missBulletHit = 0.5f;
        mechAcc = true;
        turretAcc = true;
        baseSkill = 5;
        colonistAcc = false;
        bulletSpeed = 3f;
        maxBulletSpeed = 200f;
        enemyRocket = false;
        ar_customAmmoDef = [];
        if (ModsConfig.ActiveModsInLoadOrder.Any(mod => mod.PackageId.ToLower().Contains("DualWield".ToLower())))
        {
            using_dualWeld = true;
        }

        if (ModsConfig.ActiveModsInLoadOrder.Any(mod =>
                mod.PackageId.ToLower().Contains("co.uk.epicguru.meleeanimation".ToLower())))
        {
            using_meleeAnimations = true;
        }

        if (ModsConfig.ActiveModsInLoadOrder.Any(mod =>
                mod.PackageId.ToLower().Contains("erdelf.HumanoidAlienRaces".ToLower())))
        {
            using_AlienRaces = true;
        }

        if (ModsConfig.ActiveModsInLoadOrder.Any(mod =>
                mod.PackageId.ToLower().Contains("Mlie.ShowMeYourHands".ToLower())))
        {
            using_showHands = true;
        }

        using_Oversized = AccessTools.TypeByName("CompOversizedWeapon") != null;
        if (!using_Oversized)
        {
            return;
        }

        var allWeapons = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.IsWeapon).ToList();
        foreach (var weapon in allWeapons)
        {
            saveWeaponOffsets(weapon);
        }
    }

    public override string ModIdentifier => "YayoCombat3";

    public static Vector3 GetOversizedOffset(Pawn pawn, ThingWithComps weapon)
    {
        if (!using_Oversized)
        {
            return Vector3.zero;
        }

        switch (pawn.Rotation.AsInt)
        {
            case 0:
                return northOffsets.TryGetValue(weapon.def, out var northValue)
                    ? northValue
                    : Vector3.zero;
            case 1:
                return eastOffsets.TryGetValue(weapon.def, out var eastValue)
                    ? eastValue
                    : Vector3.zero;
            case 2:
                return southOffsets.TryGetValue(weapon.def, out var southValue)
                    ? southValue
                    : Vector3.zero;
            case 3:
                return westOffsets.TryGetValue(weapon.def, out var westValue)
                    ? westValue
                    : Vector3.zero;
            default:
                return Vector3.zero;
        }
    }

    private static void saveWeaponOffsets(ThingDef weapon)
    {
        var thingComp =
            weapon.comps.FirstOrDefault(y => y.GetType().ToString().Contains("CompOversizedWeapon"));
        if (thingComp == null)
        {
            return;
        }

        var oversizedType = thingComp.GetType();
        var fields = oversizedType.GetFields().Where(info => info.Name.Contains("Offset"));

        foreach (var fieldInfo in fields)
        {
            switch (fieldInfo.Name)
            {
                case "northOffset":
                    northOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3
                        ? (Vector3)fieldInfo.GetValue(thingComp)
                        : Vector3.zero;
                    break;
                case "southOffset":
                    southOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3
                        ? (Vector3)fieldInfo.GetValue(thingComp)
                        : Vector3.zero;
                    break;
                case "westOffset":
                    westOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3
                        ? (Vector3)fieldInfo.GetValue(thingComp)
                        : Vector3.zero;
                    break;
                case "eastOffset":
                    eastOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3
                        ? (Vector3)fieldInfo.GetValue(thingComp)
                        : Vector3.zero;
                    break;
            }
        }
    }

    public override void DefsLoaded()
    {
        ammoSetting = Settings.GetHandle("ammo", "ammo_title".Translate(), "ammo_desc".Translate(), false);
        ammo = ammoSetting.Value;
        refillMechAmmoSetting = Settings.GetHandle("refillMechAmmo", "refillMechAmmo_title".Translate(),
            "refillMechAmmo_desc".Translate(), true);
        refillMechAmmo = refillMechAmmoSetting.Value;
        ammoGenSetting = Settings.GetHandle("ammoGen", "ammoGen_title".Translate(), "ammoGen_desc".Translate(), 1f);
        ammoGen = ammoGenSetting.Value;
        maxAmmoSetting = Settings.GetHandle("maxAmmo", "maxAmmo_title".Translate(), "maxAmmo_desc".Translate(), 1f);
        maxAmmo = maxAmmoSetting.Value;
        enemyAmmoSetting =
            Settings.GetHandle("enemyAmmo", "enemyAmmo_title".Translate(), "enemyAmmo_desc".Translate(), 70);
        enemyAmmo = enemyAmmoSetting.Value;
        s_enemyAmmo = enemyAmmo / 100f;
        supplyAmmoDistSetting = Settings.GetHandle("supplyAmmoDist", "supplyAmmoDist_title".Translate(),
            "supplyAmmoDist_desc".Translate(), 4);
        supplyAmmoDist = supplyAmmoDistSetting.Value;
        meleeDelaySetting = Settings.GetHandle("meleeDelay", "meleeDelayNew_title".Translate(),
            "meleeDelayNew_desc".Translate(), 0.7f);
        meleeDelay = meleeDelaySetting.Value;
        meleeRandomSetting = Settings.GetHandle("meleeRandom", "meleeRandomNew_title".Translate(),
            "meleeRandomNew_desc".Translate(), 1.3f);
        meleeRandom = meleeRandomSetting.Value;
        handProtectSetting = Settings.GetHandle("handProtect", "handProtect_title".Translate(),
            "handProtect_desc".Translate(), true);
        handProtect = handProtectSetting.Value;
        advArmorSetting =
            Settings.GetHandle("advArmor", "advArmor_title".Translate(), "advArmor_desc".Translate(), true);
        advArmor = advArmorSetting.Value;
        armorEfSetting = Settings.GetHandle("armorEf", "armorEf_title".Translate(), "armorEf_desc".Translate(), 50);
        armorEf = armorEfSetting.Value;
        s_armorEf = armorEf / 100f;
        unprotectDmgSetting = Settings.GetHandle("unprotectDmg", "unprotectDmg_title".Translate(),
            "unprotectDmg_desc".Translate(), 1.1f);
        unprotectDmg = unprotectDmgSetting.Value;
        unprotectDmgSetting.NeverVisible = true;
        advShootAccSetting = Settings.GetHandle("advShootAcc", "advShootAcc_title".Translate(),
            "advShootAcc_desc".Translate(), true);
        advShootAcc = advShootAccSetting.Value;
        accEfSetting = Settings.GetHandle("accEf", "accEf_title".Translate(), "accEf_desc".Translate(), 60);
        accEf = accEfSetting.Value;
        s_accEf = accEf / 100f;
        missBulletHitSetting = Settings.GetHandle("missBulletHit", "missBulletHit_title".Translate(),
            "missBulletHit_desc".Translate(), 50);
        missBulletHit = missBulletHitSetting.Value;
        s_missBulletHit = missBulletHit / 100f;
        mechAccSetting = Settings.GetHandle("mechAcc", "mechAcc_title".Translate(), "mechAcc_desc".Translate(), true);
        mechAcc = mechAccSetting.Value;
        turretAccSetting =
            Settings.GetHandle("turretAcc", "turretAcc_title".Translate(), "turretAcc_desc".Translate(), true);
        turretAcc = turretAccSetting.Value;
        baseSkillSetting =
            Settings.GetHandle("baseSkill", "baseSkill_title".Translate(), "baseSkill_desc".Translate(), 5);
        baseSkill = baseSkillSetting.Value;
        colonistAccSetting = Settings.GetHandle("colonistAcc", "colonistAcc_title".Translate(),
            "colonistAcc_desc".Translate(), false);
        colonistAcc = colonistAccSetting.Value;
        bulletSpeedSetting = Settings.GetHandle("bulletSpeed", "bulletSpeed_title".Translate(),
            "bulletSpeed_desc".Translate(), 3f);
        bulletSpeed = bulletSpeedSetting.Value;
        maxBulletSpeedSetting = Settings.GetHandle("maxBulletSpeed", "maxBulletSpeed_title".Translate(),
            "maxBulletSpeed_desc".Translate(), 200f);
        maxBulletSpeed = maxBulletSpeedSetting.Value;
        enemyRocketSetting = Settings.GetHandle("enemyRocket", "useRocket_title".Translate(),
            "useRocket_desc".Translate(), false);
        enemyRocket = enemyRocketSetting.Value;
        patchDef2();
    }

    public override void SettingsChanged()
    {
        ammo = ammoSetting.Value;
        ammoGen = ammoGenSetting.Value;
        maxAmmo = maxAmmoSetting.Value;
        enemyAmmoSetting.Value = Mathf.Clamp(enemyAmmoSetting.Value, 0, 500);
        enemyAmmo = enemyAmmoSetting.Value;
        s_enemyAmmo = enemyAmmo / 100f;
        supplyAmmoDist = Mathf.Clamp(supplyAmmoDistSetting.Value, -1, 100);
        meleeDelaySetting.Value = Mathf.Clamp(meleeDelaySetting.Value, 0.2f, 2f);
        meleeDelay = meleeDelaySetting.Value;
        meleeRandomSetting.Value = Mathf.Clamp(meleeRandomSetting.Value, 0f, 1.5f);
        meleeRandom = meleeRandomSetting.Value;
        handProtect = handProtectSetting.Value;
        advArmor = advArmorSetting.Value;
        armorEfSetting.Value = Mathf.Clamp(armorEfSetting.Value, 0, 100);
        armorEf = armorEfSetting.Value;
        s_armorEf = armorEf / 100f;
        unprotectDmgSetting.Value = Mathf.Clamp(unprotectDmgSetting.Value, 0.1f, 2f);
        unprotectDmg = unprotectDmgSetting.Value;
        advShootAcc = advShootAccSetting.Value;
        accEfSetting.Value = Mathf.Clamp(accEfSetting.Value, 0, 100);
        accEf = accEfSetting.Value;
        s_accEf = accEf / 100f;
        missBulletHitSetting.Value = Mathf.Clamp(missBulletHitSetting.Value, 0, 100);
        missBulletHit = missBulletHitSetting.Value;
        s_missBulletHit = missBulletHit / 100f;
        mechAcc = mechAccSetting.Value;
        turretAcc = turretAccSetting.Value;
        colonistAcc = colonistAccSetting.Value;
        baseSkillSetting.Value = Mathf.Clamp(baseSkillSetting.Value, 0, 20);
        baseSkill = baseSkillSetting.Value;
        bulletSpeedSetting.Value = Mathf.Clamp(bulletSpeedSetting.Value, 0.01f, 100f);
        bulletSpeed = bulletSpeedSetting.Value;
        maxBulletSpeedSetting.Value = Mathf.Clamp(maxBulletSpeedSetting.Value, 1f, 10000f);
        maxBulletSpeed = maxBulletSpeedSetting.Value;
        enemyRocket = enemyRocketSetting.Value;
    }

    public static void patchDef1()
    {
    }

    private static void patchDef2()
    {
        if (handProtect)
        {
            foreach (var item in DefDatabase<ThingDef>.AllDefs.Where(thing =>
                         thing.apparel is { bodyPartGroups.Count: > 0 } &&
                         (thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Hands) ||
                          thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Feet)) &&
                         !thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Torso) &&
                         !thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) &&
                         !thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.UpperHead) &&
                         !thing.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Shoulders)))
            {
                var list = new List<ApparelLayerDef>();
                foreach (var apparelLayerDef in item.apparel.layers)
                {
                    switch (apparelLayerDef.defName)
                    {
                        case "OnSkin":
                            list.Add(ApparelLayerDefOf.OnSkin_A);
                            break;
                        case "Shell":
                            list.Add(ApparelLayerDefOf.Shell_A);
                            break;
                        case "Middle":
                            list.Add(ApparelLayerDefOf.Middle_A);
                            break;
                        case "Belt":
                            list.Add(ApparelLayerDefOf.Belt_A);
                            break;
                        case "Overhead":
                            list.Add(ApparelLayerDefOf.Overhead_A);
                            break;
                    }
                }

                if (list.Count > 0)
                {
                    item.apparel.layers = list;
                }
            }

            foreach (var item2 in DefDatabase<ThingDef>.AllDefs.Where(thing => thing.apparel is
                         { bodyPartGroups.Count: > 0 }))
            {
                var bodyPartGroups = item2.apparel.bodyPartGroups;
                if (bodyPartGroups.Contains(BodyPartGroupDefOf.Arms) &&
                    !bodyPartGroups.Contains(BodyPartGroupDefOf.Hands))
                {
                    bodyPartGroups.Add(BodyPartGroupDefOf.Hands);
                }

                if (bodyPartGroups.Contains(BodyPartGroupDefOf.Legs) &&
                    !bodyPartGroups.Contains(BodyPartGroupDefOf.Feet))
                {
                    bodyPartGroups.Add(BodyPartGroupDefOf.Feet);
                }
            }
        }

        if (ammo)
        {
            var ar = new List<string>
            {
                "frost", "energy", "cryo", "gleam", "laser", "plasma", "beam", "magic", "thunder", "poison",
                "elec", "wave", "psy", "cold", "tox", "atom", "pulse", "tornado", "water", "liqu",
                "tele", "matter"
            };
            var ar2 = new List<string> { "Ice" };
            foreach (var item3 in DefDatabase<ThingDef>.AllDefs.Where(t =>
                         t.IsRangedWeapon && t.Verbs is { Count: >= 1 } && (t.modExtensions == null ||
                                                                            !t.modExtensions.Exists(x =>
                                                                                x.ToString() ==
                                                                                "HeavyWeapons.HeavyWeapon"))))
            {
                if ((int)item3.techLevel <= 1)
                {
                    continue;
                }

                var text = "";
                var verbProperties = item3.Verbs[0];
                if (verbProperties.verbClass == null || verbProperties.verbClass == typeof(Verb_ShootOneUse) ||
                    verbProperties.verbClass == typeof(Verb_LaunchProjectileStaticOneUse) ||
                    verbProperties.consumeFuelPerShot > 0f || item3.weaponTags != null &&
                    (item3.weaponTags.Contains("TurretGun") || item3.weaponTags.Contains("Artillery")))
                {
                    continue;
                }

                var compProperties_Reloadable = new CompProperties_ApparelReloadable();
                var num = verbProperties.burstShotCount /
                          ((verbProperties.ticksBetweenBurstShots * 0.016666f * verbProperties.burstShotCount) +
                           verbProperties.warmupTime +
                           item3.statBases.GetStatValueFromList(StatDefOf.RangedWeapon_Cooldown, 0f));
                var num2 = 90f;
                compProperties_Reloadable.maxCharges = Mathf.Max(3, Mathf.RoundToInt(num2 * num * maxAmmo));
                compProperties_Reloadable.ammoCountPerCharge = 1;
                compProperties_Reloadable.baseReloadTicks = Mathf.RoundToInt(60f);
                compProperties_Reloadable.soundReload = DefDatabase<SoundDef>.GetNamed("Standard_Reload");
                compProperties_Reloadable.hotKey = KeyBindingDefOf.Misc4;
                compProperties_Reloadable.chargeNoun = "ammo";
                compProperties_Reloadable.displayGizmoWhileUndrafted = true;
                if (verbProperties.defaultProjectile is { projectile.damageDef: not null })
                {
                    if (item3.weaponTags != null)
                    {
                        if (item3.weaponTags.Contains("ammo_none"))
                        {
                            continue;
                        }

                        var containStringByList = getContainStringByList("ammo_", item3.weaponTags);
                        if (containStringByList != "")
                        {
                            var array = containStringByList.Split('/');
                            text = $"yy_{array[0]}";
                            if (ThingDef.Named(text) == null)
                            {
                                text = "";
                            }

                            if (array.Length >= 2 && int.TryParse(array[1], out var result))
                            {
                                compProperties_Reloadable.maxCharges = Mathf.Max(1, Mathf.RoundToInt(result * maxAmmo));
                            }

                            if (array.Length >= 3 && int.TryParse(array[2], out var result2))
                            {
                                compProperties_Reloadable.ammoCountPerCharge = Mathf.Max(1, result2);
                            }
                        }
                    }

                    if (text == "")
                    {
                        text = "yy_ammo_";
                        var projectile = verbProperties.defaultProjectile.projectile;
                        en_ammoType ammoType;
                        if (new List<DamageDef>
                            {
                                DamageDefOf.Bomb,
                                DamageDefOf.Flame,
                                DamageDefOf.Burn
                            }.Contains(projectile.damageDef))
                        {
                            ammoType = en_ammoType.fire;
                            compProperties_Reloadable.ammoCountPerCharge =
                                Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius));
                        }
                        else if (new List<DamageDef> { DamageDefOf.Smoke }.Contains(projectile.damageDef))
                        {
                            ammoType = en_ammoType.fire;
                            compProperties_Reloadable.ammoCountPerCharge =
                                Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius / 3f));
                        }
                        else if (new List<DamageDef>
                                 {
                                     DamageDefOf.EMP,
                                     DamageDefOf.Deterioration,
                                     DamageDefOf.Extinguish,
                                     DamageDefOf.Frostbite,
                                     DamageDefOf.Rotting,
                                     DamageDefOf.Stun,
                                     DamageDefOf.TornadoScratch
                                 }.Contains(projectile.damageDef))
                        {
                            ammoType = en_ammoType.emp;
                            compProperties_Reloadable.ammoCountPerCharge =
                                Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius / 3f));
                        }
                        else if (containCheckByList(item3.defName.ToLower(), ar) ||
                                 containCheckByList(item3.defName, ar2) ||
                                 containCheckByList(projectile.damageDef.defName.ToLower(), ar) ||
                                 containCheckByList(projectile.damageDef.defName, ar2))
                        {
                            ammoType = en_ammoType.emp;
                            compProperties_Reloadable.ammoCountPerCharge =
                                Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius));
                        }
                        else if (projectile.explosionRadius > 0f)
                        {
                            compProperties_Reloadable.ammoCountPerCharge =
                                Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius));
                            ammoType = projectile.damageDef.armorCategory == null
                                ? en_ammoType.emp
                                : projectile.damageDef.armorCategory.defName switch
                                {
                                    "Sharp" => en_ammoType.fire,
                                    "Heat" => en_ammoType.fire,
                                    "Blunt" => en_ammoType.fire,
                                    _ => en_ammoType.emp
                                };
                        }
                        else if (projectile.damageDef.armorCategory != null)
                        {
                            ammoType = projectile.damageDef.armorCategory.defName switch
                            {
                                "Sharp" => en_ammoType.normal,
                                "Heat" => en_ammoType.fire,
                                "Blunt" => en_ammoType.normal,
                                _ => en_ammoType.emp
                            };
                        }
                        else
                        {
                            ammoType = en_ammoType.emp;
                            compProperties_Reloadable.ammoCountPerCharge =
                                Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius));
                        }

                        text = (int)item3.techLevel >= 5 ? $"{text}spacer" :
                            (int)item3.techLevel < 4 ? $"{text}primitive" : $"{text}industrial";
                        switch (ammoType)
                        {
                            case en_ammoType.fire:
                                text += "_fire";
                                break;
                            case en_ammoType.emp:
                                text += "_emp";
                                break;
                        }
                    }

                    compProperties_Reloadable.ammoDef = ThingDef.Named(text);
                }

                if (item3.weaponTags != null)
                {
                    var containStringByList2 = getContainStringByList("ammoDef_", item3.weaponTags);
                    if (containStringByList2 != "")
                    {
                        var array2 = containStringByList2.Split('/');
                        var array3 = array2[0].Split('_');
                        if (array3.Length >= 2)
                        {
                            compProperties_Reloadable.ammoDef = ThingDef.Named(array3[1]);
                            if (compProperties_Reloadable.ammoDef != null)
                            {
                                ar_customAmmoDef.Add(compProperties_Reloadable.ammoDef);
                            }

                            if (array2.Length >= 2 && int.TryParse(array2[1], out var result3))
                            {
                                compProperties_Reloadable.maxCharges =
                                    Mathf.Max(1, Mathf.RoundToInt(result3 * maxAmmo));
                            }

                            if (array2.Length >= 3 && int.TryParse(array2[2], out var result4))
                            {
                                compProperties_Reloadable.ammoCountPerCharge = Mathf.Max(1, result4);
                            }
                        }
                    }
                }

                if (compProperties_Reloadable.ammoDef == null)
                {
                    switch ((int)item3.techLevel)
                    {
                        case >= 5:
                            compProperties_Reloadable.ammoDef = ThingDef.Named("yy_ammo_spacer");
                            break;
                        case >= 4:
                            compProperties_Reloadable.ammoDef = ThingDef.Named("yy_ammo_industrial");
                            break;
                        default:
                            compProperties_Reloadable.ammoDef = ThingDef.Named("yy_ammo_primitive");
                            break;
                    }
                }

                item3.comps.Add(compProperties_Reloadable);
            }

            foreach (var item4 in DefDatabase<RecipeDef>.AllDefs.Where(thing => thing.defName.Contains("yy_ammo")))
            {
                if (item4.products is { Count: > 0 })
                {
                    item4.products[0].count = Mathf.RoundToInt(item4.products[0].count * ammoGen);
                }
            }
        }
        else
        {
            foreach (var item5 in DefDatabase<RecipeDef>.AllDefs.Where(thing => thing.defName.Contains("yy_ammo")))
            {
                item5.recipeUsers = [];
            }

            foreach (var item6 in DefDatabase<ThingDef>.AllDefs.Where(thing => thing.defName.Contains("yy_ammo")))
            {
                item6.tradeability = Tradeability.None;
                item6.tradeTags = null;
            }
        }

        if (advArmor)
        {
            foreach (var item7 in DefDatabase<PawnKindDef>.AllDefs.Where(pawn =>
                         pawn.defaultFactionDef == FactionDefOf.Mechanoid))
            {
                item7.race.SetStatBaseValue(StatDefOf.ArmorRating_Sharp,
                    item7.race.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp) * 1.3f);
                item7.combatPower *= 1.3f;
            }
        }

        var thingDef = ThingDef.Named("Gun_AntiArmor_Rocket");
        if (!enemyRocket)
        {
            thingDef.weaponTags = [];
        }
    }

    private static bool containCheckByList(string origin, List<string> ar)
    {
        foreach (var value in ar)
        {
            if (origin.Contains(value))
            {
                return true;
            }
        }

        return false;
    }

    private static string getContainStringByList(string keyword, List<string> ar)
    {
        foreach (var containStringByList in ar)
        {
            if (containStringByList.Contains(keyword))
            {
                return containStringByList;
            }
        }

        return "";
    }

    private enum en_ammoType
    {
        normal,
        fire,
        emp
    }
}