using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using Verse;

namespace yayoCombat;

// Static core class: holds flags and main logic converted from old ModBase
[StaticConstructorOnStartup]
public static class YayoCombatCore
{
    private static int accEf = 60;
    private static int armorEf = 50;
    private static readonly Dictionary<ThingDef, Vector3> eastOffsets = new();
    private static int enemyAmmo = 70;
    private static bool enemyRocket = false;
    private static bool handProtect = true;
    private static float maxAmmo = 1f;
    private static int missBulletHit = 50;
    private static readonly Dictionary<ThingDef, Vector3> northOffsets = new();

    // Offsets for oversized weapons
    private static readonly Dictionary<ThingDef, Vector3> southOffsets = new();
    private static readonly bool using_Oversized;
    private static readonly Dictionary<ThingDef, Vector3> westOffsets = new();
    public static bool advArmor = true;
    public static bool advShootAcc = true;
    public static bool ammo = false;
    public static float ammoGen = 1f;
    public static readonly List<ThingDef> ar_customAmmoDef = new();
    public static int baseSkill = 5;
    public static float bulletSpeed = 3f;
    public static bool colonistAcc = false;
    public static float maxBulletSpeed = 200f;
    public static bool mechAcc = true;
    public static float meleeDelay = 0.7f;
    public static float meleeRandom = 1.3f;
    public static bool refillMechAmmo = true;
    public static float s_accEf = 0.6f;
    public static float s_armorEf = 0.5f;
    public static float s_enemyAmmo = 0.7f;
    public static float s_missBulletHit = 0.5f;
    public static int supplyAmmoDist = 4;
    public static bool turretAcc = true;
    public static float unprotectDmg = 1.1f;
    public static readonly bool using_AlienRaces;
    // Feature flags
    public static readonly bool using_dualWeld;
    public static readonly bool using_meleeAnimations;
    public static readonly bool using_showHands;

    // Public state fields (mirrors previous static fields)
    public static Dictionary<Thing, Tuple<Vector3, float>> weaponLocations;


    static YayoCombatCore()
    {
        using_dualWeld = false;
        using_meleeAnimations = false;
        using_AlienRaces = false;
        using_showHands = false;

        // Detect mods in load order (rough equivalent to HugsLib check)
        if(ModsConfig.ActiveModsInLoadOrder
            .Any(mod => mod.PackageId != null && mod.PackageId.ToLower().Contains("dualwield")))
        {
            using_dualWeld = true;
        }

        if(ModsConfig.ActiveModsInLoadOrder
            .Any(mod => mod.PackageId != null && mod.PackageId.ToLower().Contains("co.uk.epicguru.meleeanimation")))
        {
            using_meleeAnimations = true;
        }

        if(ModsConfig.ActiveModsInLoadOrder
            .Any(mod => mod.PackageId != null && mod.PackageId.ToLower().Contains("humanoidalienraces")))
        {
            using_AlienRaces = true;
        }

        if(ModsConfig.ActiveModsInLoadOrder
            .Any(mod => mod.PackageId != null && mod.PackageId.ToLower().Contains("showmeyourhands")))
        {
            using_showHands = true;
        }

        using_Oversized = AccessTools.TypeByName("CompOversizedWeapon") != null;
        if(using_Oversized)
        {
            // Cache per-weapon offsets
            try
            {
                var allWeapons = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.IsWeapon).ToList();
                foreach(var weapon in allWeapons)
                {
                    saveWeaponOffsets(weapon);
                }
            } catch(Exception ex)
            {
                Log.Warning($"YayoCombatCore: error caching oversized offsets: {ex}");
            }
        }
        importOldHugsLibSettings();

        ApplySettingsFrom(YayoCombatMod.Instance.Settings);

        new Harmony("Mlie.YayosCombat3").PatchAll(Assembly.GetExecutingAssembly());
    }

    private static bool containCheckByList(string origin, List<string> ar)
    {
        foreach(var value in ar)
        {
            if(origin.Contains(value))
            {
                return true;
            }
        }

        return false;
    }

    private static string getContainStringByList(string keyword, List<string> ar)
    {
        foreach(var containStringByList in ar)
        {
            if(containStringByList.Contains(keyword))
            {
                return containStringByList;
            }
        }

        return string.Empty;
    }

    private static void importOldHugsLibSettings()
    {
        var hugsLibConfig = Path.Combine(GenFilePaths.SaveDataFolderPath, "HugsLib", "ModSettings.xml");
        if (!new FileInfo(hugsLibConfig).Exists)
        {
            return;
        }

        var xml = XDocument.Load(hugsLibConfig);
        var modNodeName = "YayoCombat3";

        var modSettings = xml.Root?.Element(modNodeName);
        if (modSettings == null)
        {
            return;
        }

        foreach (var modSetting in modSettings.Elements())
        {
            if (modSetting.Name == "ammo")
            {
                YayoCombatMod.Instance.Settings.ammo = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "refillMechAmmo")
            {
                YayoCombatMod.Instance.Settings.refillMechAmmo = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "ammoGen")
            {
                YayoCombatMod.Instance.Settings.ammoGen = float.Parse(modSetting.Value);
            }
            if (modSetting.Name == "maxAmmo")
            {
                YayoCombatMod.Instance.Settings.maxAmmo = float.Parse(modSetting.Value);
            }
            if (modSetting.Name == "enemyAmmo")
            {
                YayoCombatMod.Instance.Settings.enemyAmmo = int.Parse(modSetting.Value);
            }
            if (modSetting.Name == "supplyAmmoDist")
            {
                YayoCombatMod.Instance.Settings.supplyAmmoDist = int.Parse(modSetting.Value);
            }
            if (modSetting.Name == "meleeDelay")
            {
                YayoCombatMod.Instance.Settings.meleeDelay = float.Parse(modSetting.Value);
            }
            if (modSetting.Name == "meleeRandom")
            {
                YayoCombatMod.Instance.Settings.meleeRandom = float.Parse(modSetting.Value);
            }
            if (modSetting.Name == "handProtect")
            {
                YayoCombatMod.Instance.Settings.handProtect = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "advArmor")
            {
                YayoCombatMod.Instance.Settings.advArmor = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "armorEf")
            {
                YayoCombatMod.Instance.Settings.armorEf = int.Parse(modSetting.Value);
            }
            if (modSetting.Name == "unprotectDmg")
            {
                YayoCombatMod.Instance.Settings.unprotectDmg = float.Parse(modSetting.Value);
            }
            if (modSetting.Name == "advShootAcc")
            {
                YayoCombatMod.Instance.Settings.advShootAcc = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "accEf")
            {
                YayoCombatMod.Instance.Settings.accEf = int.Parse(modSetting.Value);
            }
            if (modSetting.Name == "missBulletHit")
            {
                YayoCombatMod.Instance.Settings.missBulletHit = int.Parse(modSetting.Value);
            }
            if (modSetting.Name == "mechAcc")
            {
                YayoCombatMod.Instance.Settings.mechAcc = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "turretAcc")
            {
                YayoCombatMod.Instance.Settings.turretAcc = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "baseSkill")
            {
                YayoCombatMod.Instance.Settings.baseSkill = int.Parse(modSetting.Value);
            }
            if (modSetting.Name == "colonistAcc")
            {
                YayoCombatMod.Instance.Settings.colonistAcc = bool.Parse(modSetting.Value);
            }
            if (modSetting.Name == "bulletSpeed")
            {
                YayoCombatMod.Instance.Settings.bulletSpeed = float.Parse(modSetting.Value);
            }
            if (modSetting.Name == "maxBulletSpeed")
            {
                YayoCombatMod.Instance.Settings.maxBulletSpeed = float.Parse(modSetting.Value);
            }
            if (modSetting.Name == "enemyRocket")
            {
                YayoCombatMod.Instance.Settings.enemyRocket = bool.Parse(modSetting.Value);
            }
        }

        xml.Root.Element(modNodeName)?.Remove();
        xml.Save(hugsLibConfig);

        Log.Message("[YayoCombat3]: Imported old HugLib-settings");
    }

    private static void saveWeaponOffsets(ThingDef weapon)
    {
        try
        {
            var thingComp = weapon.comps.FirstOrDefault(y => y.GetType().ToString().Contains("CompOversizedWeapon"));
            if(thingComp == null)
            {
                return;
            }

            var oversizedType = thingComp.GetType();
            var fields = oversizedType.GetFields().Where(info => info.Name.Contains("Offset"));

            foreach(var fieldInfo in fields)
            {
                switch(fieldInfo.Name)
                {
                    case "northOffset":
                        northOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3 v1 ? v1 : Vector3.zero;
                        break;
                    case "southOffset":
                        southOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3 v2 ? v2 : Vector3.zero;
                        break;
                    case "westOffset":
                        westOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3 v3 ? v3 : Vector3.zero;
                        break;
                    case "eastOffset":
                        eastOffsets[weapon] = fieldInfo.GetValue(thingComp) is Vector3 v4 ? v4 : Vector3.zero;
                        break;
                }
            }
        } catch(Exception ex)
        {
            Log.Warning($"YayoCombatCore.saveWeaponOffsets error for {weapon?.defName}: {ex}");
        }
    }

    // Public method to apply the def modifications that original patchDef2 did
    public static void ApplyDefPatches()
    {
        try
        {
            // HAND PROTECT: modify apparel layer defs and body part groups
            if(handProtect)
            {
                foreach(var item in DefDatabase<ThingDef>.AllDefs
                    .Where(
                        thing => thing.apparel is { bodyPartGroups.Count: > 0 } &&
                            (thing.apparel.bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Hands) ||
                                thing.apparel.bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Feet)) &&
                            !thing.apparel.bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Torso) &&
                            !thing.apparel.bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.FullHead) &&
                            !thing.apparel.bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.UpperHead) &&
                            !thing.apparel.bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Shoulders)))
                {
                    var list = new List<ApparelLayerDef>();
                    foreach(var apparelLayerDef in item.apparel.layers)
                    {
                        switch(apparelLayerDef.defName)
                        {
                            case "OnSkin":
                                list.Add(yayoCombat_Defs.ApparelLayerDefOf.OnSkin_A);
                                break;
                            case "Shell":
                                list.Add(yayoCombat_Defs.ApparelLayerDefOf.Shell_A);
                                break;
                            case "Middle":
                                list.Add(yayoCombat_Defs.ApparelLayerDefOf.Middle_A);
                                break;
                            case "Belt":
                                list.Add(yayoCombat_Defs.ApparelLayerDefOf.Belt_A);
                                break;
                            case "Overhead":
                                list.Add(yayoCombat_Defs.ApparelLayerDefOf.Overhead_A);
                                break;
                        }
                    }

                    if(list.Count > 0)
                    {
                        item.apparel.layers = list;
                    }
                }

                foreach(var item2 in DefDatabase<ThingDef>.AllDefs
                    .Where(
                        thing => thing.apparel is
                    { bodyPartGroups.Count: > 0 }))
                {
                    var bodyPartGroups = item2.apparel.bodyPartGroups;
                    if(bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Arms) &&
                        !bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Hands))
                    {
                        bodyPartGroups.Add(yayoCombat_Defs.BodyPartGroupDefOf.Hands);
                    }

                    if(bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Legs) &&
                        !bodyPartGroups.Contains(yayoCombat_Defs.BodyPartGroupDefOf.Feet))
                    {
                        bodyPartGroups.Add(yayoCombat_Defs.BodyPartGroupDefOf.Feet);
                    }
                }
            }

            // AMMO: add reloadable comp to ranged weapons, adjust recipes
            if(ammo)
            {
                var ar = new List<string>
                {
                    "frost",
                    "energy",
                    "cryo",
                    "gleam",
                    "laser",
                    "plasma",
                    "beam",
                    "magic",
                    "thunder",
                    "poison",
                    "elec",
                    "wave",
                    "psy",
                    "cold",
                    "tox",
                    "atom",
                    "pulse",
                    "tornado",
                    "water",
                    "liqu",
                    "tele",
                    "matter"
                };
                var ar2 = new List<string> { "Ice" };

                foreach(var item3 in DefDatabase<ThingDef>.AllDefs
                    .Where(
                        t => t.IsRangedWeapon &&
                            t.Verbs is { Count: >= 1 } &&
                            (t.modExtensions == null ||
                                !t.modExtensions.Exists(x => x.ToString() == "HeavyWeapons.HeavyWeapon"))))
                {
                    // skip very low tech weapons
                    if((int)item3.techLevel <= 1)
                    {
                        continue;
                    }

                    var text = string.Empty;
                    var verbProperties = item3.Verbs[0];

                    // skip one-use shoot verbs, fuel-consumption or turret/artillery tags
                    if(verbProperties.verbClass == null ||
                        verbProperties.verbClass == typeof(Verb_ShootOneUse) ||
                        verbProperties.verbClass == typeof(Verb_LaunchProjectileStaticOneUse) ||
                        verbProperties.consumeFuelPerShot > 0f ||
                        (item3.weaponTags != null &&
                            (item3.weaponTags.Contains("TurretGun") || item3.weaponTags.Contains("Artillery"))))
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

                    if(verbProperties.defaultProjectile is { projectile.damageDef: not null })
                    {
                        var projectile = verbProperties.defaultProjectile.projectile;
                        if(item3.weaponTags != null)
                        {
                            if(item3.weaponTags.Contains("ammo_none"))
                            {
                                continue;
                            }

                            var containStringByList = getContainStringByList("ammo_", item3.weaponTags);
                            if(containStringByList != string.Empty)
                            {
                                var array = containStringByList.Split('/');
                                text = $"yy_{array[0]}";
                                if(ThingDef.Named(text) == null)
                                {
                                    text = string.Empty;
                                }

                                if(array.Length >= 2 && int.TryParse(array[1], out var result))
                                {
                                    compProperties_Reloadable.maxCharges = Mathf.Max(
                                        1,
                                        Mathf.RoundToInt(result * maxAmmo));
                                }

                                if(array.Length >= 3 && int.TryParse(array[2], out var result2))
                                {
                                    compProperties_Reloadable.ammoCountPerCharge = Mathf.Max(1, result2);
                                }
                            }
                        }

                        if(text == string.Empty)
                        {
                            text = "yy_ammo_";
                            en_ammoType ammoType;
                            if(new List<DamageDef> { DamageDefOf.Bomb, DamageDefOf.Flame, DamageDefOf.Burn }.Contains(
                                projectile.damageDef))
                            {
                                ammoType = en_ammoType.fire;
                                compProperties_Reloadable.ammoCountPerCharge =
                                        Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius));
                            } else if(new List<DamageDef> { DamageDefOf.Smoke }.Contains(projectile.damageDef))
                            {
                                ammoType = en_ammoType.fire;
                                compProperties_Reloadable.ammoCountPerCharge =
                                        Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius / 3f));
                            } else if(new List<DamageDef>
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
                            } else if(containCheckByList(item3.defName.ToLower(), ar) ||
                                containCheckByList(item3.defName, ar2) ||
                                containCheckByList(projectile.damageDef.defName.ToLower(), ar) ||
                                containCheckByList(projectile.damageDef.defName, ar2))
                            {
                                ammoType = en_ammoType.emp;
                                compProperties_Reloadable.ammoCountPerCharge =
                                        Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius));
                            } else if(projectile.explosionRadius > 0f)
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
                            } else if(projectile.damageDef.armorCategory != null)
                            {
                                ammoType = projectile.damageDef.armorCategory.defName switch
                                {
                                    "Sharp" => en_ammoType.normal,
                                    "Heat" => en_ammoType.fire,
                                    "Blunt" => en_ammoType.normal,
                                    _ => en_ammoType.emp
                                };
                            } else
                            {
                                ammoType = en_ammoType.emp;
                                compProperties_Reloadable.ammoCountPerCharge =
                                        Mathf.Max(1, Mathf.RoundToInt(projectile.explosionRadius));
                            }

                            text = (int)item3.techLevel >= 5
                                ? $"{text}spacer"
                                : (int)item3.techLevel < 4 ? $"{text}primitive" : $"{text}industrial";
                            switch(ammoType)
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

                    // allow specific ammo override via weapon tags
                    if(item3.weaponTags != null)
                    {
                        var containStringByList2 = getContainStringByList("ammoDef_", item3.weaponTags);
                        if(containStringByList2 != string.Empty)
                        {
                            var array2 = containStringByList2.Split('/');
                            var array3 = array2[0].Split('_');
                            if(array3.Length >= 2)
                            {
                                compProperties_Reloadable.ammoDef = ThingDef.Named(array3[1]);
                                if(compProperties_Reloadable.ammoDef != null)
                                {
                                    ar_customAmmoDef.Add(compProperties_Reloadable.ammoDef);
                                }

                                if(array2.Length >= 2 && int.TryParse(array2[1], out var result3))
                                {
                                    compProperties_Reloadable.maxCharges =
                                            Mathf.Max(1, Mathf.RoundToInt(result3 * maxAmmo));
                                }

                                if(array2.Length >= 3 && int.TryParse(array2[2], out var result4))
                                {
                                    compProperties_Reloadable.ammoCountPerCharge = Mathf.Max(1, result4);
                                }
                            }
                        }
                    }

                    // fallback ammo defs by tech level
                    if(compProperties_Reloadable.ammoDef == null)
                    {
                        switch((int)item3.techLevel)
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

                // scale recipes that produce yy_ammo
                foreach(var item4 in DefDatabase<RecipeDef>.AllDefs.Where(thing => thing.defName.Contains("yy_ammo")))
                {
                    if(item4.products is { Count: > 0 })
                    {
                        item4.products[0].count = Mathf.RoundToInt(item4.products[0].count * ammoGen);
                    }
                }
            } else // ammo disabled -> remove yy_ammo usage from recipe users and disable trade
            {
                foreach(var item5 in DefDatabase<RecipeDef>.AllDefs.Where(thing => thing.defName.Contains("yy_ammo")))
                {
                    item5.recipeUsers = new List<ThingDef>();
                }

                foreach(var item6 in DefDatabase<ThingDef>.AllDefs.Where(thing => thing.defName.Contains("yy_ammo")))
                {
                    item6.tradeability = Tradeability.None;
                    item6.tradeTags = null;
                }
            }

            // ADV ARMOR: buff mech pawn kinds
            if(advArmor)
            {
                foreach(var item7 in DefDatabase<PawnKindDef>.AllDefs
                    .Where(pawn => pawn.defaultFactionDef == FactionDefOf.Mechanoid))
                {
                    item7.race
                        .SetStatBaseValue(
                            StatDefOf.ArmorRating_Sharp,
                            item7.race.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp) * 1.3f);
                    item7.combatPower *= 1.3f;
                }
            }

            // enemy rockets toggle
            var thingDef = ThingDef.Named("Gun_AntiArmor_Rocket");
            if(!enemyRocket)
            {
                thingDef.weaponTags = new List<string>();
            }
        } catch(Exception ex)
        {
            Log.Warning($"YayoCombatCore.ApplyDefPatches exception: {ex}");
        }
    }


    public static void ApplySettingsFrom(YayoCombatSettings s)
    {
        if(s == null)
        {
            return;
        }

        ammo = s.ammo;
        refillMechAmmo = s.refillMechAmmo;
        ammoGen = Mathf.Max(0.0001f, s.ammoGen);
        maxAmmo = Mathf.Max(0.0001f, s.maxAmmo);
        enemyAmmo = Mathf.Clamp(s.enemyAmmo, 0, 500);
        s_enemyAmmo = enemyAmmo / 100f;
        supplyAmmoDist = Mathf.Clamp(s.supplyAmmoDist, -1, 100);

        meleeDelay = Mathf.Clamp(s.meleeDelay, 0.2f, 2f);
        meleeRandom = Mathf.Clamp(s.meleeRandom, 0f, 1.5f);

        handProtect = s.handProtect;
        advArmor = s.advArmor;
        armorEf = Mathf.Clamp(s.armorEf, 0, 100);
        s_armorEf = armorEf / 100f;
        unprotectDmg = Mathf.Clamp(s.unprotectDmg, 0.1f, 2f);

        advShootAcc = s.advShootAcc;
        accEf = Mathf.Clamp(s.accEf, 0, 100);
        s_accEf = accEf / 100f;
        missBulletHit = Mathf.Clamp(s.missBulletHit, 0, 100);
        s_missBulletHit = missBulletHit / 100f;

        mechAcc = s.mechAcc;
        turretAcc = s.turretAcc;
        colonistAcc = s.colonistAcc;
        baseSkill = Mathf.Clamp(s.baseSkill, 0, 20);
        bulletSpeed = Mathf.Clamp(s.bulletSpeed, 0.01f, 100f);
        maxBulletSpeed = Mathf.Clamp(s.maxBulletSpeed, 1f, 10000f);
        enemyRocket = s.enemyRocket;
    }

    // Public accessor for oversized offsets (keeps previous API)
    public static Vector3 GetOversizedOffset(Pawn pawn, ThingWithComps weapon)
    {
        if(!using_Oversized || pawn == null || weapon == null)
        {
            return Vector3.zero;
        }

        switch(pawn.Rotation.AsInt)
        {
            case 0:
                return northOffsets.TryGetValue(weapon.def, out var northValue) ? northValue : Vector3.zero;
            case 1:
                return eastOffsets.TryGetValue(weapon.def, out var eastValue) ? eastValue : Vector3.zero;
            case 2:
                return southOffsets.TryGetValue(weapon.def, out var southValue) ? southValue : Vector3.zero;
            case 3:
                return westOffsets.TryGetValue(weapon.def, out var westValue) ? westValue : Vector3.zero;
            default:
                return Vector3.zero;
        }
    }

    private enum en_ammoType
    {
        normal,
        fire,
        emp
    }
}
