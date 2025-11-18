using HarmonyLib;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat;

public class YayoCombatMod : Mod
{
    private static string currentVersion;

    private Vector2 scrollPosition = Vector2.zero;
    public YayoCombatSettings Settings;
    public static YayoCombatMod Instance;

    public YayoCombatMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<YayoCombatSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
        Instance = this;

        new Harmony("Mlie.YayosCombat3").PatchAll();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        float scrollHeight = 1000f;
        Rect viewRect = new Rect(0f, 0f, inRect.width - 20f, scrollHeight);
        Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

        Listing_Standard listing = new Listing_Standard();
        listing.Begin(viewRect);

        // --- Reset All Button ---
        Rect resetRect = listing.GetRect(Text.LineHeight);
        if(Widgets.ButtonText(resetRect, "reset_all".Translate()))
        {
            Settings.ResetToDefaults();
        }
        listing.Gap(12f);

        // === Ammo & Reloading ===
        listing.Label("ammoandreloading".Translate());

        listing.CheckboxLabeled("ammo_title".Translate(), ref Settings.ammo, "ammo_desc".Translate());
        listing.CheckboxLabeled(
            "refillMechAmmo_title".Translate(),
            ref Settings.refillMechAmmo,
            "refillMechAmmo_desc".Translate());
        listing.Gap(6f);

        listing.TextFieldNumericLabeledWithTooltip(
            "ammoGen_title".Translate(),
            "ammoGen_desc".Translate(),
            ref Settings.ammoGen,
            ref Settings.ammoGenBuffer,
            0,
            10000);

        listing.TextFieldNumericLabeledWithTooltip(
            "maxAmmo_title".Translate(),
            "maxAmmo_desc".Translate(),
            ref Settings.maxAmmo,
            ref Settings.maxAmmoBuffer,
            0,
            10000);

        listing.TextFieldNumericLabeledWithTooltip(
            "enemyAmmo_title".Translate(),
            "enemyAmmo_desc".Translate(),
            ref Settings.enemyAmmo,
            ref Settings.enemyAmmoBuffer,
            0,
            500);

        listing.TextFieldNumericLabeledWithTooltip(
            "supplyAmmoDist_title".Translate(),
            "supplyAmmoDist_desc".Translate(),
            ref Settings.supplyAmmoDist,
            ref Settings.supplyAmmoDistBuffer,
            -1,
            100);

        listing.GapLine();

        // === Melee ===
        listing.Label("melee".Translate());

        listing.TextFieldNumericLabeledWithTooltip(
            "meleeDelay_title".Translate(),
            "meleeDelay_desc".Translate(),
            ref Settings.meleeDelay,
            ref Settings.meleeDelayBuffer,
            0.1f,
            5f);

        listing.TextFieldNumericLabeledWithTooltip(
            "meleeRandom_title".Translate(),
            "meleeRandom_desc".Translate(),
            ref Settings.meleeRandom,
            ref Settings.meleeRandomBuffer,
            0f,
            3f);

        listing.GapLine();

        // === Armor / Accuracy ===
        listing.Label("armornaccuracy".Translate());

        listing.CheckboxLabeled(
            "handProtect_title".Translate(),
            ref Settings.handProtect,
            "handProtect_desc".Translate());
        listing.CheckboxLabeled("advArmor_title".Translate(), ref Settings.advArmor, "advArmor_desc".Translate());

        listing.TextFieldNumericLabeledWithTooltip(
            "armorEf_title".Translate(),
            "armorEf_desc".Translate(),
            ref Settings.armorEf,
            ref Settings.armorEfBuffer,
            0,
            100);

        listing.TextFieldNumericLabeledWithTooltip(
            "unprotectDmg_title".Translate(),
            "unprotectDmg_desc".Translate(),
            ref Settings.unprotectDmg,
            ref Settings.unprotectDmgBuffer,
            0.1f,
            5f);

        listing.GapLine();

        // === Shooting accuracy ===
        listing.Label("shootingAccuracy".Translate());

        listing.CheckboxLabeled(
            "advShootAcc_title".Translate(),
            ref Settings.advShootAcc,
            "advShootAcc_desc".Translate());

        listing.TextFieldNumericLabeledWithTooltip(
            "accEf_title".Translate(),
            "accEf_desc".Translate(),
            ref Settings.accEf,
            ref Settings.accEfBuffer,
            0,
            100);

        listing.TextFieldNumericLabeledWithTooltip(
            "missBulletHit_title".Translate(),
            "missBulletHit_desc".Translate(),
            ref Settings.missBulletHit,
            ref Settings.missBulletHitBuffer,
            0,
            100);

        listing.CheckboxLabeled("mechAcc_title".Translate(), ref Settings.mechAcc, "mechAcc_desc".Translate());
        listing.CheckboxLabeled("turretAcc_title".Translate(), ref Settings.turretAcc, "turretAcc_desc".Translate());
        listing.CheckboxLabeled(
            "colonistAcc_title".Translate(),
            ref Settings.colonistAcc,
            "colonistAcc_desc".Translate());

        listing.GapLine();

        // === Misc ===
        listing.Label("misc".Translate());

        listing.TextFieldNumericLabeledWithTooltip(
            "baseSkill_title".Translate(),
            "baseSkill_desc".Translate(),
            ref Settings.baseSkill,
            ref Settings.baseSkillBuffer,
            0,
            20);

        listing.TextFieldNumericLabeledWithTooltip(
            "bulletSpeed_title".Translate(),
            "bulletSpeed_desc".Translate(),
            ref Settings.bulletSpeed,
            ref Settings.bulletSpeedBuffer,
            0.01f,
            100f);

        listing.TextFieldNumericLabeledWithTooltip(
            "maxBulletSpeed_title".Translate(),
            "maxBulletSpeed_desc".Translate(),
            ref Settings.maxBulletSpeed,
            ref Settings.maxBulletSpeedBuffer,
            1f,
            10000f);

        listing.CheckboxLabeled("useRocket_title".Translate(), ref Settings.enemyRocket, "useRocket_desc".Translate());
        if(currentVersion != null)
        {
            listing.Gap();
            GUI.contentColor = Color.gray;
            listing.Label("ycombat_CurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }
        listing.End();
        Widgets.EndScrollView();
    }

    public override string SettingsCategory() { return "ycModName".Translate(); }

    public override void WriteSettings()
    {
        base.WriteSettings();
        YayoCombatCore.ApplySettingsFrom(Settings);
    }
}
