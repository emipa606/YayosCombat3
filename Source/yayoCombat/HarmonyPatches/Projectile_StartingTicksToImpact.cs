using HarmonyLib;
using UnityEngine;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Projectile), "StartingTicksToImpact", MethodType.Getter)]
internal static class Projectile_StartingTicksToImpact
{
    public static bool Prefix(Projectile __instance, Vector3 ___origin, Vector3 ___destination, ref float __result)
    {
        if (__instance.def.projectile.flyOverhead ||
            __instance.def.projectile.speed <= 23f && __instance.def.projectile.explosionDelay <= 0)
        {
            return true;
        }

        var num = __instance.def.projectile.SpeedTilesPerTick * YayoCombatCore.bulletSpeed;
        if (num >= YayoCombatCore.maxBulletSpeed * 0.01f)
        {
            num = YayoCombatCore.maxBulletSpeed * 0.01f;
        }

        var num2 = (___origin - ___destination).magnitude / num;
        if (num2 <= 0f)
        {
            num2 = 0.001f;
        }

        __result = num2;
        return false;
    }
}