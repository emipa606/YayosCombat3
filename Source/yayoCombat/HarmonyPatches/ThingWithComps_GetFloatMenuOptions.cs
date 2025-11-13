using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.GetFloatMenuOptions))]
internal class ThingWithComps_GetFloatMenuOptions
{
    [HarmonyPriority(0)]
    private static void Postfix(ref IEnumerable<FloatMenuOption> __result, ThingWithComps __instance, Pawn selPawn)
    {
        if (!YayoCombatCore.ammo)
        {
            return;
        }

        var compApparelReloadable = __instance.TryGetComp<CompApparelReloadable>();
        if (selPawn.IsColonist && compApparelReloadable is { AmmoDef: not null } &&
            !compApparelReloadable.Props.destroyOnEmpty &&
            compApparelReloadable.RemainingCharges > 0)
        {
            __result = new List<FloatMenuOption>
            {
                new(
                    "eject_AmmoAmount".Translate(compApparelReloadable.RemainingCharges,
                        compApparelReloadable.AmmoDef.LabelCap), cleanWeapon, MenuOptionPriority.High)
            };
        }

        return;

        void cleanWeapon()
        {
            reloadUtility.EjectAmmo(selPawn, __instance);
        }
    }
}