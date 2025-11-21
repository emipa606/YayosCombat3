using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(StatPart_ReloadMarketValue), "TransformAndExplain")]
public static class StatPart_ReloadMarketValue_TransformAndExplain
{
    public static bool Prefix(StatRequest req, ref float val, StringBuilder explanation)
    {
        if (req.Thing is not { def.IsRangedWeapon: true })
        {
            return true;
        }

        var compApparelReloadable = req.Thing.TryGetComp<CompApparelReloadable>();
        if (compApparelReloadable == null)
        {
            return true;
        }

        if (compApparelReloadable.AmmoDef == null || compApparelReloadable.RemainingCharges == 0)
        {
            return false;
        }

        var remainingCharges = compApparelReloadable.RemainingCharges;
        var num = compApparelReloadable.AmmoDef.BaseMarketValue * remainingCharges;
        val += num;
        explanation?.AppendLine(
            "StatsReport_ReloadMarketValue".Translate(compApparelReloadable.AmmoDef.Named("AMMO"),
                remainingCharges.Named("COUNT")) + ": " + num.ToStringMoneyOffset());

        return false;
    }
}