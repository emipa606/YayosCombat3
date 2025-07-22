//using System.Collections.Generic;
//using HarmonyLib;
//using RimWorld;
//using RimWorld.Utility;
//using Verse;

//namespace yayoCombat.HarmonyPatches;

//[HarmonyPatch(typeof(ReloadableUtility), nameof(ReloadableUtility.FindPotentiallyReloadableGear))]
//internal class ReloadableUtility_FindPotentiallyReloadableGear
//{
//    private static IEnumerable<Pair<CompApparelReloadable, Thing>> Postfix(
//        IEnumerable<Pair<CompApparelReloadable, Thing>> __result, Pawn pawn, List<Thing> potentialAmmo)
//    {
//        foreach (var pair in __result)
//        {
//            yield return pair;
//        }

//        if (!yayoCombat.ammo)
//        {
//            yield break;
//        }

//        if (pawn.equipment == null)
//        {
//            yield break;
//        }

//        foreach (var thing in pawn.equipment.AllEquipmentListForReading)
//        {
//            var comp = thing.TryGetComp<CompApparelReloadable>();
//            if (comp?.AmmoDef == null)
//            {
//                continue;
//            }

//            foreach (var ammoThing in potentialAmmo)
//            {
//                if (ammoThing?.def == comp.Props.ammoDef)
//                {
//                    yield return new Pair<CompApparelReloadable, Thing>(comp, ammoThing);
//                }
//            }
//        }
//    }
//}

