using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace yayoCombat;

public class JobDriver_EjectAmmo : JobDriver
{
    private ThingWithComps Gear => (ThingWithComps)job.GetTarget(TargetIndex.A).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        pawn.Reserve(Gear, job);
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var f = this;
        Thing gear = f.Gear;
        GetActor();
        var comp = gear?.TryGetComp<CompApparelReloadable>();
        f.FailOn(() => comp == null);
        f.FailOn(() => comp is { RemainingCharges: <= 0 });
        f.FailOnDestroyedOrNull(TargetIndex.A);
        f.FailOnIncapable(PawnCapacityDefOf.Manipulation);
        var getNextIngredient = Toils_General.Label();
        yield return getNextIngredient;
        foreach (var item in f.ejectAsMuchAsPossible(comp))
        {
            yield return item;
        }

        yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
        yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, true)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A);
        yield return Toils_Jump.JumpIf(getNextIngredient, () => !job.GetTargetQueue(TargetIndex.A).NullOrEmpty());
        foreach (var item2 in f.ejectAsMuchAsPossible(comp))
        {
            yield return item2;
        }

        yield return new Toil
        {
            initAction = delegate
            {
                var carriedThing = pawn.carryTracker.CarriedThing;
                if (carriedThing is { Destroyed: false })
                {
                    pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Near, out _);
                }
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }

    private IEnumerable<Toil> ejectAsMuchAsPossible(CompApparelReloadable comp)
    {
        var done = Toils_General.Label();
        yield return Toils_Jump.JumpIf(done,
            () => pawn.carryTracker.CarriedThing == null ||
                  pawn.carryTracker.CarriedThing.stackCount < comp.MinAmmoNeeded(true));
        yield return Toils_General.Wait(comp.Props.baseReloadTicks).WithProgressBarToilDelay(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate { reloadUtility.EjectAmmoAction(GetActor(), comp); },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        yield return done;
    }
}