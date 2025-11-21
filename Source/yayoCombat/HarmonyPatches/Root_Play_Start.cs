using HarmonyLib;
using Verse;

namespace yayoCombat.HarmonyPatches;

[HarmonyPatch(typeof(Root_Play), nameof(Root_Play.Start))]
public static class Root_Play_Start
{
    public static void Prefix()
    {
        YayoCombatCore.ApplyDefPatches(); 
    }
}
