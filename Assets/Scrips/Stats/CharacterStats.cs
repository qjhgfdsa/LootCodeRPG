using UnityEngine;

namespace SA
{
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Current")]
        public float _health;
        public float _focus;
        public float _stamina;
        public int _souls;

        public float _healthRecovery = 60;
        public float _focusRecovery = 70;

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
        public int physical = 87;
        public int vs_strike = 87;
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

        public void InitCurrent()
        {

            if (statEffect != null)
            {
                statEffect();
            }

            //  _health = hp;
            //  _focus = fp;
            //  _stamina = stamina;

        }
        public delegate void StatEffect();
        public StatEffect statEffect;
        public void AddHealth()
        {
            hp += 5;

        }
        public void RemoveHealth()
        {
            hp -= 5;
        }
    }

    public enum AttributeType
    {
        vigor, endurance, vitality, strength, dexterity, intelligence, faith, luck, 
        hp, fp, stamina, equip_load, poise, item_discover, attunement,level, 
    }

    public enum AttackDefenseType
    {
        physical, strike, slash, thrust, magic, fire, lightning, dark, stability, bleed, curse, frost,poison,
       
    }
    public enum WeaponDamageType
    {
        sum, vs_strike, vs_slash, vs_thrust,
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
        public string weaponId;
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


