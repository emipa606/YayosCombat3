using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(CompApparelReloadable), nameof(CompApparelReloadable.CompTick))]
public static class CompApparelReloadable_CompTick
{
    public static void Prefix(CompApparelReloadable __instance, ref int ___replenishInTicks)
    {
        var itemName = __instance.parent?.def?.label ?? "Unknown";
        var hasAmmo = __instance.AmmoDef != null;
        var shouldReplenish = __instance.Props.replenishAfterCooldown;
        var timerAt = ___replenishInTicks;

        // Prevent automatic replenishment for items without ammunition (like deadlife packs)
        // This fixes the infinite charge bug where non-ammo items would replenish instantly
        if (hasAmmo || !shouldReplenish || timerAt > 0)
        {
            return;
        }

        Log.Message(
            $"[YayoCombat] BLOCKING replenishment for {itemName}: AmmoDef=NULL, replenishAfterCooldown={true}, timer={timerAt}. Charges before: {__instance.RemainingCharges}"
        );
        // Disable replenishment by setting replenishInTicks to a large number
        // This prevents the charges from being reset to max
        ___replenishInTicks = int.MaxValue;
        Log.Message($"[YayoCombat] Set timer to MaxValue for {itemName}");
    }
}