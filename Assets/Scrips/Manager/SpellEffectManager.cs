using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

namespace SA
{
    public class SpellEffectManager : MonoBehaviour
    {
        Dictionary<string, int> s_effects = new Dictionary<string, int>();

        public void UseSpellEffect(string id, StateManager c, EnemyStates e = null)
        {
            int index = GetEffectId(id);

            if (index == -1)
            {
                Debug.Log("เวทย์นี้ไม่มี");
                return;
            }

            switch (index)
            {
                case 0:
                    FireBreath(c);
                    break;
                case 1:
                    FireShield(c);
                    break;
                case 2:
                    HealingSmall(c);
                    break;
                case 3:
                    Fireball(c);
                    break;
                case 4:
                    OnFire(c, e);
                    break;
            }
        }

        int GetEffectId(string id)
        {
            int index = -1;

            if (s_effects.TryGetValue(id, out index))
            {
                return index;
            }
            return index;
        }

        void FireBreath(StateManager c)
        {
            c.spellCast_start = c.inventoryManager.OpenBreathCollider;
            c.spellCast_loop = c.inventoryManager.EmitSpellParticle;
            c.spellCast_loop += c.SubstractFocusOverTime;

            c.spellCast_stop = c.inventoryManager.CloseBreathCollider;
        }
        void FireShield(StateManager c)
        {
            c.spellCast_start = c.inventoryManager.OpenBlockCollider;
            c.spellCast_loop = c.inventoryManager.EmitSpellParticle;
            c.spellCast_loop += c.SubstractFocusOverTime;
            c.spellCast_loop += c.AffectBlocking;
            c.spellCast_stop = c.inventoryManager.CloseBlockCollider;
            c.spellCast_stop += c.StopAffectinBlocking;
        }
        void HealingSmall(StateManager c)
        {
            c.spellCast_loop = c.AddHealth;
        }

        void Fireball(StateManager c)
        {
            c.spellCast_loop = c.spellCast_loop = c.inventoryManager.EmitSpellParticle;
        }
        void OnFire(StateManager c, EnemyStates e)
        {
            if (c != null)
            {

            }
            if (e != null)
            {
                e.spellEffect_loop = e.OnFire;
            }
        }

        public static SpellEffectManager singleton;
        void Awake()
        {
            singleton = this;

            s_effects.Add("firebreath", 0);
            s_effects.Add("fireshield", 1);
            s_effects.Add("healingSmall", 2);
            s_effects.Add("fireball", 3);
            s_effects.Add("onFire", 4);
        }
    }
}
