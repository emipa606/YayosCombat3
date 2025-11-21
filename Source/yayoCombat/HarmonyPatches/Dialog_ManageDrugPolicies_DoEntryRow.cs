using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Dialog_ManageDrugPolicies), "DoEntryRow")]
public static class Dialog_ManageDrugPolicies_DoEntryRow
{
    public static bool Prefix(Rect rect, DrugPolicyEntry entry)
    {
        if (!YayoCombatCore.ammo)
        {
            return true;
        }

        calculateColumnsWidths(rect, out var addictionWidth, out var allowJoyWidth, out var scheduledWidth,
            out var drugNameWidth, out var frequencyWidth, out var moodThresholdWidth, out var joyThresholdWidth,
            out var takeToInventoryWidth);
        Text.Anchor = TextAnchor.MiddleLeft;
        var outerRect = new Rect(rect.x + 10f, rect.y, 30f, 30f);
        var num = outerRect.size.x + 20f;
        var num2 = rect.x + num;
        drugNameWidth -= num;
        Widgets.DrawTextureFitted(outerRect, entry.drug.uiIcon, 1f);
        Widgets.Label(new Rect(num2, rect.y, drugNameWidth, rect.height).ContractedBy(4f), entry.drug.LabelCap);
        Widgets.InfoCardButton(num2 + drugNameWidth - 35f, rect.y + (float)((rect.height - 24.0) / 2.0), entry.drug);
        var num3 = num2 + drugNameWidth - 10f;
        if (entry.drug.IsDrug)
        {
            Widgets.TextFieldNumeric(new Rect(num3, rect.y, takeToInventoryWidth, rect.height).ContractedBy(4f),
                ref entry.takeToInventory, ref entry.takeToInventoryTempBuffer, 0f, 15f);
        }
        else
        {
            Widgets.TextFieldNumeric(new Rect(num3, rect.y, takeToInventoryWidth + 118f, rect.height).ContractedBy(4f),
                ref entry.takeToInventory, ref entry.takeToInventoryTempBuffer, 0f, 5000f);
        }

        var num4 = num3 + takeToInventoryWidth + 10f;
        if (entry.drug.IsAddictiveDrug)
        {
            Widgets.Checkbox(num4, rect.y, ref entry.allowedForAddiction, 24f, false, true);
        }

        var num5 = num4 + addictionWidth;
        if (entry.drug.IsPleasureDrug)
        {
            Widgets.Checkbox(num5, rect.y, ref entry.allowedForJoy, 24f, false, true);
        }

        var num6 = num5 + allowJoyWidth;
        if (entry.drug.IsDrug)
        {
            Widgets.Checkbox(num6, rect.y, ref entry.allowScheduled, 24f, false, true);
        }

        var num7 = num6 + scheduledWidth;
        if (entry.allowScheduled)
        {
            entry.daysFrequency = Widgets.FrequencyHorizontalSlider(
                new Rect(num7, rect.y, frequencyWidth, rect.height).ContractedBy(4f), entry.daysFrequency, 0.1f, 25f,
                true);
            var num8 = num7 + frequencyWidth;
            string label = entry.onlyIfMoodBelow >= 1.0
                ? "NoDrugUseRequirement".Translate()
                : entry.onlyIfMoodBelow.ToStringPercent();
            entry.onlyIfMoodBelow =
                Widgets.HorizontalSlider(
                    new Rect(num8, rect.y, moodThresholdWidth, rect.height).ContractedBy(4f),
                    entry.onlyIfMoodBelow, 0.01f, 1f, true, label);
            var num9 = num8 + moodThresholdWidth;
            string label2 = entry.onlyIfJoyBelow >= 1.0
                ? "NoDrugUseRequirement".Translate()
                : entry.onlyIfJoyBelow.ToStringPercent();
            entry.onlyIfJoyBelow =
                Widgets.HorizontalSlider(
                    new Rect(num9, rect.y, joyThresholdWidth, rect.height).ContractedBy(4f),
                    entry.onlyIfJoyBelow, 0.01f, 1f, true, label2);
        }

        Text.Anchor = TextAnchor.UpperLeft;
        return false;
    }

    private static void calculateColumnsWidths(Rect rect, out float addictionWidth, out float allowJoyWidth,
        out float scheduledWidth, out float drugNameWidth, out float frequencyWidth, out float moodThresholdWidth,
        out float joyThresholdWidth, out float takeToInventoryWidth)
    {
        var num = rect.width - 108f;
        drugNameWidth = num * 0.3f;
        addictionWidth = 36f;
        allowJoyWidth = 36f;
        scheduledWidth = 36f;
        frequencyWidth = num * 0.35f;
        moodThresholdWidth = num * 0.15f;
        joyThresholdWidth = num * 0.15f;
        takeToInventoryWidth = num * 0.05f;
    }
}