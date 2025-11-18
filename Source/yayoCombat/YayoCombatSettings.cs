using Verse;

namespace yayoCombat
{
    public class YayoCombatSettings : ModSettings
    {
        public string ammoGenBuffer, maxAmmoBuffer, enemyAmmoBuffer, supplyAmmoDistBuffer;
        public string meleeDelayBuffer, meleeRandomBuffer;
        public string armorEfBuffer, unprotectDmgBuffer, accEfBuffer, missBulletHitBuffer;
        public string baseSkillBuffer, bulletSpeedBuffer, maxBulletSpeedBuffer;


        public bool ammo = false;
        public bool refillMechAmmo = true;
        public float ammoGen = 1f;
        public float maxAmmo = 1f;
        public int enemyAmmo = 70;
        public int supplyAmmoDist = 4;

        public float meleeDelay = 0.7f;
        public float meleeRandom = 1.3f;

        public bool handProtect = true;
        public bool advArmor = true;
        public int armorEf = 50;
        public float unprotectDmg = 1.1f;

        public bool advShootAcc = true;
        public int accEf = 60;
        public int missBulletHit = 50;
        public bool mechAcc = true;
        public bool turretAcc = true;
        public int baseSkill = 5;
        public bool colonistAcc = false;
        public float bulletSpeed = 3f;
        public float maxBulletSpeed = 200f;
        public bool enemyRocket = false;


        public void ResetToDefaults()
        {
            ammo = true;
            refillMechAmmo = true;
            ammoGen = 1;
            maxAmmo = 1;
            enemyAmmo = 100;
            supplyAmmoDist = 25;

            meleeDelay = 1f;
            meleeRandom = 0.5f;

            handProtect = true;
            advArmor = true;
            armorEf = 100;
            unprotectDmg = 1f;

            advShootAcc = true;
            accEf = 100;
            missBulletHit = 0;
            mechAcc = true;
            turretAcc = true;
            colonistAcc = true;

            baseSkill = 8;
            bulletSpeed = 30f;
            maxBulletSpeed = 3000f;
            enemyRocket = true;

        }

        public void ClearBuffers()
        {
           
            ammoGenBuffer = maxAmmoBuffer = enemyAmmoBuffer = supplyAmmoDistBuffer = "";
            meleeDelayBuffer = meleeRandomBuffer = "";
            armorEfBuffer = unprotectDmgBuffer = "";
            accEfBuffer = missBulletHitBuffer = "";
            baseSkillBuffer = bulletSpeedBuffer = maxBulletSpeedBuffer = "";
        }


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
    }
}
