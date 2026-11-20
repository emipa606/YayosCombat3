using AlienRace;
using UnityEngine;
using Verse;

namespace yayoCombat;

public static class AlienRaceUtility
{
    public static Vector2 AlienRacesPatch(Pawn pawn, Thing eq)
    {
        var def = pawn.def;
        if (def is not ThingDef_AlienRace alien)
        {
            return new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
        }

        return alien.alienRace.generalSettings.alienPartGenerator.customDrawSize;
    }
}