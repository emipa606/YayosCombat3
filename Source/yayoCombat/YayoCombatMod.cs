using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using ApparelLayerDefOf = yayoCombat_Defs.ApparelLayerDefOf;
using BodyPartGroupDefOf = yayoCombat_Defs.BodyPartGroupDefOf;

namespace yayoCombat
{
   
    public class YayoCombatMod : Mod
    {
        private YayoCombatSettings settings;

        
        private Vector2 scrollPosition = Vector2.zero;

        public YayoCombatMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<YayoCombatSettings>();

            new Harmony("Mlie.YayosCombat3").PatchAll();
            
        }

        public override string SettingsCategory() => "ycModName".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var settings = GetSettings<YayoCombatSettings>();

            float scrollHeight = 1000f; 
            Rect viewRect = new Rect(0f, 0f, inRect.width - 20f, scrollHeight);
            Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);

            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            // --- Reset All Button ---
            Rect resetRect = listing.GetRect(Text.LineHeight);
            if (Widgets.ButtonText(resetRect, "reset_all".Translate()))
            {
                settings.ResetToDefaults();
                // Clear all buffers if needed
                settings.ClearBuffers();
            }
            listing.Gap(12f);

            // === Ammo & Reloading ===
            listing.Label("ammoandreloading".Translate());

            listing.CheckboxLabeled("ammo_title".Translate(),
                ref settings.ammo, "ammo_desc".Translate());
            listing.CheckboxLabeled("refillMechAmmo_title".Translate(),
                ref settings.refillMechAmmo, "refillMechAmmo_desc".Translate());
            listing.Gap(6f);

            listing.TextFieldNumericLabeledWithTooltip(
                "ammoGen_title".Translate(),
                "ammoGen_desc".Translate(),
                ref settings.ammoGen, ref settings.ammoGenBuffer,
                0, 10000);

            listing.TextFieldNumericLabeledWithTooltip(
                "maxAmmo_title".Translate(),
                "maxAmmo_desc".Translate(),
                ref settings.maxAmmo, ref settings.maxAmmoBuffer,
                0, 10000);

            listing.TextFieldNumericLabeledWithTooltip(
                "enemyAmmo_title".Translate(),
                "enemyAmmo_desc".Translate(),
                ref settings.enemyAmmo, ref settings.enemyAmmoBuffer,
                0, 500);

            listing.TextFieldNumericLabeledWithTooltip(
                "supplyAmmoDist_title".Translate(),
                "supplyAmmoDist_desc".Translate(),
                ref settings.supplyAmmoDist, ref settings.supplyAmmoDistBuffer,
                -1, 100);

            listing.GapLine();

            // === Melee ===
            listing.Label("melee".Translate());

            listing.TextFieldNumericLabeledWithTooltip(
                "meleeDelay_title".Translate(),
                "meleeDelay_desc".Translate(),
                ref settings.meleeDelay, ref settings.meleeDelayBuffer,
                0.1f, 5f);

            listing.TextFieldNumericLabeledWithTooltip(
                "meleeRandom_title".Translate(),
                "meleeRandom_desc".Translate(),
                ref settings.meleeRandom, ref settings.meleeRandomBuffer,
                0f, 3f);

            listing.GapLine();

            // === Armor / Accuracy ===
            listing.Label("armornaccuracy".Translate());

            listing.CheckboxLabeled("handProtect_title".Translate(),
                ref settings.handProtect, "handProtect_desc".Translate());
            listing.CheckboxLabeled("advArmor_title".Translate(),
                ref settings.advArmor, "advArmor_desc".Translate());

            listing.TextFieldNumericLabeledWithTooltip(
                "armorEf_title".Translate(),
                "armorEf_desc".Translate(),
                ref settings.armorEf, ref settings.armorEfBuffer,
                0, 100);

            listing.TextFieldNumericLabeledWithTooltip(
                "unprotectDmg_title".Translate(),
                "unprotectDmg_desc".Translate(),
                ref settings.unprotectDmg, ref settings.unprotectDmgBuffer,
                0.1f, 5f);

            listing.GapLine();

            // === Shooting accuracy ===
            listing.Label("shootingAccuracy".Translate());

            listing.CheckboxLabeled("advShootAcc_title".Translate(),
                ref settings.advShootAcc, "advShootAcc_desc".Translate());

            listing.TextFieldNumericLabeledWithTooltip(
                "accEf_title".Translate(),
                "accEf_desc".Translate(),
                ref settings.accEf, ref settings.accEfBuffer,
                0, 100);

            listing.TextFieldNumericLabeledWithTooltip(
                "missBulletHit_title".Translate(),
                "missBulletHit_desc".Translate(),
                ref settings.missBulletHit, ref settings.missBulletHitBuffer,
                0, 100);

            listing.CheckboxLabeled("mechAcc_title".Translate(),
                ref settings.mechAcc, "mechAcc_desc".Translate());
            listing.CheckboxLabeled("turretAcc_title".Translate(),
                ref settings.turretAcc, "turretAcc_desc".Translate());
            listing.CheckboxLabeled("colonistAcc_title".Translate(),
                ref settings.colonistAcc, "colonistAcc_desc".Translate());

            listing.GapLine();

            // === Misc ===
            listing.Label("misc".Translate());

            listing.TextFieldNumericLabeledWithTooltip(
                "baseSkill_title".Translate(),
                "baseSkill_desc".Translate(),
                ref settings.baseSkill, ref settings.baseSkillBuffer,
                0, 20);

            listing.TextFieldNumericLabeledWithTooltip(
                "bulletSpeed_title".Translate(),
                "bulletSpeed_desc".Translate(),
                ref settings.bulletSpeed, ref settings.bulletSpeedBuffer,
                0.01f, 100f);

            listing.TextFieldNumericLabeledWithTooltip(
                "maxBulletSpeed_title".Translate(),
                "maxBulletSpeed_desc".Translate(),
                ref settings.maxBulletSpeed, ref settings.maxBulletSpeedBuffer,
                1f, 10000f);

            listing.CheckboxLabeled("useRocket_title".Translate(),
                ref settings.enemyRocket, "useRocket_desc".Translate());

            listing.End();
            Widgets.EndScrollView();

            WriteSettings();
            YayoCombatCore.ApplySettingsFrom(settings);
        }






        public override void WriteSettings()
        {
            base.WriteSettings();          
            YayoCombatCore.ApplySettingsFrom(settings);
        }
    }   
       

    
}
