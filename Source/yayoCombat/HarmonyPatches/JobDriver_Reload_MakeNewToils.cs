using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Utility;
using Verse;
using Verse.AI;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(JobDriver_Reload), "MakeNewToils")]
internal class JobDriver_Reload_MakeNewToils
{
    private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_Reload __instance, Pawn ___pawn,
        Job ___job)
    {
        if (!yayoCombat.ammo)
        {
            foreach (var value in values)
            {
                yield return value;
            }

            yield break;
        }

        var gear = ___job.GetTarget(TargetIndex.A).Thing;
        IReloadableComp apparelComp = gear?.TryGetComp<CompApparelReloadable>();
        IReloadableComp abilityComp = gear?.TryGetComp<CompEquippableAbilityReloadable>();

        if (apparelComp == null || abilityComp == null)
        {
            foreach (var value in values)
            {
                yield return value;
            }

            yield break;
        }

        List<IReloadableComp> reloadables = [apparelComp, abilityComp];


        __instance.FailOn(() => reloadables.Any(comp => ReloadableUtility.OwnerOf(comp) != ___pawn));
        __instance.FailOn(() => !reloadables.Any(comp => comp.NeedsReload(true)));
        __instance.FailOnDestroyedOrNull(TargetIndex.A);
        __instance.FailOnIncapable(PawnCapacityDefOf.Manipulation);
        var getNextIngredient = Toils_General.Label();
        yield return getNextIngredient;
        foreach (var item in ReloadAsMuchAsPossible(reloadables, ___pawn))
        {
            yield return item;
        }

        yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B);
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
        yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B);
        yield return Toils_Jump.JumpIf(getNextIngredient, () => !___job.GetTargetQueue(TargetIndex.B).NullOrEmpty());
        foreach (var item2 in ReloadAsMuchAsPossible(reloadables, ___pawn))
        {
            yield return item2;
        }

        var toil = ToilMaker.MakeToil("MakeNewToils");
        toil.initAction = delegate
        {
            var carriedThing = ___pawn.carryTracker.CarriedThing;
            if (carriedThing is { Destroyed: false })
            {
                ___pawn.carryTracker.TryDropCarriedThing(___pawn.Position, ThingPlaceMode.Near, out _);
            }
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return toil;
    }

    private static IEnumerable<Toil> ReloadAsMuchAsPossible(List<IReloadableComp> reloadables, Pawn pawn)
    {
        var delay = (int)reloadables.Average(comp => comp.BaseReloadTicks);
        var done = Toils_General.Label();
        yield return Toils_Jump.JumpIf(done,
            () =>
            {
                var carriedThing = pawn.carryTracker.CarriedThing;
                if (carriedThing == null)
                {
                    return true;
                }

                return pawn.carryTracker.CarriedThing.stackCount <
                       reloadables.First(comp => comp.AmmoDef == pawn.carryTracker.CarriedThing.def)
                           .MinAmmoNeeded(true);
            });

        yield return Toils_General.Wait(delay).WithProgressBarToilDelay(TargetIndex.A);
        var toil = ToilMaker.MakeToil();
        toil.initAction = delegate
        {
            var carriedThing = pawn.carryTracker.CarriedThing;
            reloadables.First(comp => comp.AmmoDef == carriedThing.def).ReloadFrom(carriedThing);
        };
        toil.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return toil;
        yield return done;
    }
}