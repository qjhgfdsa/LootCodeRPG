using UnityEngine;
using System.Collections.Generic;

namespace SA
{
    public class ItemEffectManager : MonoBehaviour
    {
        Dictionary<string, int> i_effects = new Dictionary<string, int>();

        void initEffectsId()
        {
            i_effects.Add("bestus", 0);
            i_effects.Add("bestus_focus", 1);
            i_effects.Add("souls", 2);
        }

        public void UseItemEffect(string effectId, StateManager states)
        {
            int i = GetEffectId(effectId);
            if (i < 0)
            {
                Debug.Log("ItemEffectManager: Effect not found");
                return;
            }
            switch (i)
            {
                case 0:
                    AddHealth(states);
                    break;
                case 1:
                    AddFocus(states);
                    break;
                case 2:
                    AddSoul(states);
                    break;
            }
        }

        #region Add any Consumable Effect i want to add here
        void AddHealth(StateManager states)
        {
            states.characterStats._health += states.characterStats._healthRecovery;

        }
        void AddFocus(StateManager states)
        {
            states.characterStats._focus += states.characterStats._focusRecovery;
        }
        void AddSoul(StateManager states)
        {
            states.characterStats._souls += 100;
        }
        #endregion
        int GetEffectId(string id)
        {
            int index = -1;
            if (i_effects.TryGetValue(id, out index))
            {
                return index;
            }
            return index;
        }

        public static ItemEffectManager singleton;
        void Awake()
        {
            singleton = this;
            initEffectsId();
        }
    }
}
