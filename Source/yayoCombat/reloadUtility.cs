using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using JobDefOf = yayoCombat_Defs.JobDefOf;

namespace yayoCombat;

internal class reloadUtility
{
    internal static void EjectAmmo(Pawn pawn, ThingWithComps t)
    {
        if (!pawn.IsColonist && pawn.equipment.Primary == null)
        {
            return;
        }

        var job = new Job(JobDefOf.EjectAmmo, t)
        {
            count = 1
        };
        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }

    internal static void EjectAmmoAction(Pawn p, CompApparelReloadable cp)
    {
        var num = 0;
        while (cp.RemainingCharges > 0)
        {
            cp.UsedOnce();
            num++;
        }

        while (num > 0)
        {
            var thing = ThingMaker.MakeThing(cp.AmmoDef);
            thing.stackCount = Mathf.Min(thing.def.stackLimit, num) * cp.Props.ammoCountPerCharge;
            num -= thing.stackCount;
            GenPlace.TryPlaceThing(thing, p.Position, p.MapHeld, ThingPlaceMode.Near);
        }

        cp.Props.soundReload.PlayOneShot(new TargetInfo(p.Position, p.MapHeld));
    }

    internal static void TryThingEjectAmmoDirect(Thing w, bool forbidden = false, Pawn pawn = null)
    {
        if (!w.def.IsWeapon || w.TryGetComp<CompApparelReloadable>() == null)
        {
            return;
        }

        var CompApparelReloadable = w.TryGetComp<CompApparelReloadable>();
        var num = 0;
        while (CompApparelReloadable.RemainingCharges > 0)
        {
            CompApparelReloadable.UsedOnce();
            num++;
        }

        while (num > 0)
        {
            var thing = ThingMaker.MakeThing(CompApparelReloadable.AmmoDef);
            thing.stackCount = Mathf.Min(thing.def.stackLimit, num) * CompApparelReloadable.Props.ammoCountPerCharge;
            if (forbidden)
            {
                thing.SetForbidden(true);
            }

            num -= thing.stackCount;
            if (pawn != null)
            {
                GenPlace.TryPlaceThing(thing, pawn.Position, pawn.MapHeld, ThingPlaceMode.Near);
            }
            else
            {
                GenPlace.TryPlaceThing(thing, w.Position, w.MapHeld, ThingPlaceMode.Near);
            }
        }
    }

    public static void TryAutoReload(CompApparelReloadable cp)
    {
        if (cp == null || cp.RemainingCharges > 0 || cp.AmmoDef == null)
        {
            return;
        }

        var p = cp.Wearer;
        if (p?.inventory?.innerContainer == null || p.Dead)
        {
            return;
        }

        var inventory = p.inventory.innerContainer.ToList();
        var thingsToReload = new List<Thing>();
        foreach (var thing in inventory)
        {
            if (thing != null && thing.def == cp.AmmoDef)
            {
                thingsToReload.Add(thing);
            }
        }


        if (thingsToReload.Count == 0 && !p.RaceProps.Humanlike && YayoCombatCore.refillMechAmmo)
        {
            var thing = ThingMaker.MakeThing(cp.AmmoDef);
            thing.stackCount = cp.MaxAmmoNeeded(true);
            p.inventory.innerContainer.TryAdd(thing);
            thingsToReload.Add(thing);
        }

        if (thingsToReload.Count > 0)
        {
            var reloadedThings = new List<Thing>();
            var maxAmmoNeeded = cp.MaxAmmoNeeded(true);
            for (var i = thingsToReload.Count - 1; i >= 0; i--)
            {
                var ammoToUse = Mathf.Min(maxAmmoNeeded, thingsToReload[i].stackCount);
                if (!p.inventory.innerContainer.TryDrop(thingsToReload[i], p.Position, p.MapHeld, ThingPlaceMode.Direct,
                        ammoToUse, out var resultingThing) && !p.inventory.innerContainer.TryDrop(thingsToReload[i],
                        p.Position, p.MapHeld, ThingPlaceMode.Near, ammoToUse, out resultingThing))
                {
                    continue; // cant generate item?
                }

                if (resultingThing != null && ammoToUse > 0)
                {
                    maxAmmoNeeded -= ammoToUse;
                    reloadedThings.Add(resultingThing);
                }

                if (maxAmmoNeeded <= 0)
                {
                    break;
                }
            }

            if (reloadedThings.Count <= 0)
            {
                return;
            }

            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Reload, cp.ReloadableThing); // cp.parent ?
            job.targetQueueB = reloadedThings.Select(t => new LocalTargetInfo(t)).ToList();
            job.count = reloadedThings.Sum(t => t.stackCount);
            job.count = Math.Min(job.count, cp.MaxAmmoNeeded(true));
            p.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            p.jobs.jobQueue.EnqueueLast(JobMaker.MakeJob(RimWorld.JobDefOf.Goto, p.Position));
            return;
        }

        if (YayoCombatCore.supplyAmmoDist < 0)
        {
            return;
        }

        List<Thing> reservableThings = null;
        try
        {
            reservableThings = RefuelWorkGiverUtility.FindEnoughReservableThings(
                desiredQuantity: new IntRange(cp.MinAmmoNeeded(false), cp.MaxAmmoNeeded(false)), pawn: p,
                rootCell: p.Position,
                validThing: t => t.def == cp.AmmoDef && p.Position.DistanceTo(t.Position) <= YayoCombatCore.supplyAmmoDist);
        }
        catch (Exception ex)
        {
            Log.ErrorOnce($"{p} cannot find ammo for {cp}\n{ex}", ex.GetHashCode());
        }

        if (reservableThings == null || p.jobs.jobQueue.ToList().Count > 0)
        {
            return;
        }

        p.jobs.TryTakeOrderedJob(JobGiver_Reload.MakeReloadJob(cp, reservableThings), JobTag.Misc);
        p.jobs.jobQueue.EnqueueLast(JobMaker.MakeJob(RimWorld.JobDefOf.Goto, p.Position));
    }
}