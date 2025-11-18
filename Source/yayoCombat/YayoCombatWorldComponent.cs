using RimWorld.Planet;


namespace yayoCombat;


public class YayoCombatWorldComponent : WorldComponent
{
    public YayoCombatWorldComponent(World world) : base(world) { YayoCombatCore.ApplyDefPatches(); }

    public override void ExposeData() { base.ExposeData(); }
}
