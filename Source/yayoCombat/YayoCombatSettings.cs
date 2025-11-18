using Verse;

namespace yayoCombat;

public class YayoCombatSettings : ModSettings
{
    public int accEf = 60;
    public bool advArmor = true;

    public bool advShootAcc = true;


    public bool ammo = false;
    public float ammoGen = 1f;
    public string ammoGenBuffer, maxAmmoBuffer, enemyAmmoBuffer, supplyAmmoDistBuffer;
    public int armorEf = 50;
    public string armorEfBuffer, unprotectDmgBuffer, accEfBuffer, missBulletHitBuffer;
    public int baseSkill = 5;
    public string baseSkillBuffer, bulletSpeedBuffer, maxBulletSpeedBuffer;
    public float bulletSpeed = 3f;
    public bool colonistAcc = false;
    public int enemyAmmo = 70;
    public bool enemyRocket = false;

    public bool handProtect = true;
    public float maxAmmo = 1f;
    public float maxBulletSpeed = 200f;
    public bool mechAcc = true;

    public float meleeDelay = 0.7f;
    public string meleeDelayBuffer, meleeRandomBuffer;
    public float meleeRandom = 1.3f;
    public int missBulletHit = 50;
    public bool refillMechAmmo = true;
    public int supplyAmmoDist = 4;
    public bool turretAcc = true;
    public float unprotectDmg = 1.1f;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ammo, "ammo", false);
        Scribe_Values.Look(ref refillMechAmmo, "refillMechAmmo", true);
        Scribe_Values.Look(ref ammoGen, "ammoGen", 1f);
        Scribe_Values.Look(ref maxAmmo, "maxAmmo", 1f);
        Scribe_Values.Look(ref enemyAmmo, "enemyAmmo", 70);
        Scribe_Values.Look(ref supplyAmmoDist, "supplyAmmoDist", 4);

        Scribe_Values.Look(ref meleeDelay, "meleeDelay", 0.7f);
        Scribe_Values.Look(ref meleeRandom, "meleeRandom", 1.3f);

        Scribe_Values.Look(ref handProtect, "handProtect", true);
        Scribe_Values.Look(ref advArmor, "advArmor", true);
        Scribe_Values.Look(ref armorEf, "armorEf", 50);
        Scribe_Values.Look(ref unprotectDmg, "unprotectDmg", 1.1f);

        Scribe_Values.Look(ref advShootAcc, "advShootAcc", true);
        Scribe_Values.Look(ref accEf, "accEf", 60);
        Scribe_Values.Look(ref missBulletHit, "missBulletHit", 50);
        Scribe_Values.Look(ref mechAcc, "mechAcc", true);
        Scribe_Values.Look(ref turretAcc, "turretAcc", true);
        Scribe_Values.Look(ref baseSkill, "baseSkill", 5);
        Scribe_Values.Look(ref colonistAcc, "colonistAcc", false);
        Scribe_Values.Look(ref bulletSpeed, "bulletSpeed", 3f);
        Scribe_Values.Look(ref maxBulletSpeed, "maxBulletSpeed", 200f);
        Scribe_Values.Look(ref enemyRocket, "enemyRocket", false);

        base.ExposeData();
    }


    public void ResetToDefaults()
    {
        ammo = true;
        refillMechAmmo = true;
        ammoGen = 1;
        ammoGenBuffer = "1";
        maxAmmo = 1;
        maxAmmoBuffer = "1";
        enemyAmmo = 70;
        enemyAmmoBuffer = "70";
        supplyAmmoDist = 4;
        supplyAmmoDistBuffer = "4";

        meleeDelay = 0.7f;
        meleeDelayBuffer = "0.7";
        meleeRandom = 1.3f;
        meleeRandomBuffer = "1.3";

        handProtect = true;
        advArmor = true;
        armorEf = 50;
        armorEfBuffer = "50";
        unprotectDmg = 1.1f;
        unprotectDmgBuffer = "1.1";

        advShootAcc = true;
        accEf = 60;
        accEfBuffer = "60";
        missBulletHit = 50;
        missBulletHitBuffer = "50";
        mechAcc = true;
        turretAcc = true;
        colonistAcc = true;

        baseSkill = 5;
        baseSkillBuffer = "5";
        bulletSpeed = 3f;
        bulletSpeedBuffer = "3";
        maxBulletSpeed = 200f;
        maxBulletSpeedBuffer = "200";
        enemyRocket = true;
    }
}
