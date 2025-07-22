using HarmonyLib;
using RimWorld;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(CompApparelReloadable), nameof(CompApparelReloadable.UsedOnce))]
internal class CompApparelReloadable_UsedOnce
{
    private static void Postfix(CompApparelReloadable __instance)
    {
        if (!yayoCombat.ammo || __instance.Wearer == null)
        {
            return;
        }

        // 남은 탄약이 0 일경우 게임튕김 방지를 위해 사냥 중지
        if (__instance.RemainingCharges == 0)
        {
            if (__instance.Wearer.CurJobDef == JobDefOf.Hunt)
            {
                __instance.Wearer.jobs.StopAll();
            }
        }

        // 알아서 장전 ai
        reloadUtility.TryAutoReload(__instance);
    }
}