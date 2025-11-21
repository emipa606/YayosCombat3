using HarmonyLib;
using RimWorld;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.MakeGun))]
public static class Building_TurretGun_MakeGun
{
    public static bool Prefix(Building_TurretGun __instance)
    {
        if (!YayoCombatCore.ammo)
        {
            return true;
        }

        var turretGunDef = __instance.def.building.turretGunDef;
        var removedReload = false;
        for (var i = 0; i < turretGunDef.comps.Count; i++)
        {
            if (turretGunDef.comps[i].compClass != typeof(CompApparelReloadable))
            {
                continue;
            }

            turretGunDef.comps.Remove(turretGunDef.comps[i]);
            removedReload = true;
        }

        if (removedReload)
        {
            __instance.def.building.turretGunDef = turretGunDef;
        }

        return true;
    }
}