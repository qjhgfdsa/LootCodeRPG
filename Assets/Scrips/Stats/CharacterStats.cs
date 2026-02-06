using UnityEngine;

namespace SA
{
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Base Power")]
        public int hp = 100;
        public int fp = 100;
        public int stamina = 100;
        public float poise = 20;
        public int itemDiscover = 111;

        [Header("Attack Power")]
        public int R_weapon_1 = 51;
        public int R_weapon_2 = 51;
        public int R_weapon_3 = 51;
        public int L_weapon_1 = 51;
        public int L_weapon_2 = 51;
        public int L_weapon_3 = 51;

        [Header("Defense Power")]
        public int Physical = 87;
        public int vs_strikes = 87;
        public int vs_slash = 87;
        public int vs_thrust = 87;
        public int magic = 30;
        public int fire = 30;
        public int lightning = 30;
        public int dark = 30;

        [Header("Resistances")]
        public int bleed = 100;
        public int frost = 100;
        public int poison = 100;
        public int curse = 100;

        public int attunemntSlots = 0;
    }

    [System.Serializable]
    public class Attributes
    {
        public int level = 1;
        public int souls = 0;
        public int vigor = 11;
        public int attunement = 11;
        public int endurance = 11;
        public int vitality = 11;
        public int strength = 11;
        public int dexterity = 11;
        public int intelligence = 11;
        public int faith = 11;
        public int luck = 11;
    }

    [System.Serializable]
    public class WeaponStats
    {
        public int physical;
        public int strike;
        public int slash;
        public int thrust;
        public int magic = 0;
        public int fire = 0;
        public int lightning = 0;
        public int dark = 0;

    }
}


