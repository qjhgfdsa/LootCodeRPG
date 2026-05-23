using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();
        public ItemAction consumableItem;


        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            UpdateActionsOneHanded();
        }

        public void UpdateActionsOneHanded()
        {
            if (states == null || states.inventoryManager == null)
            {
                Debug.LogWarning("ActionManager: inventoryManager is not ready, skip one-handed action refresh.");
                return;
            }

            if (states.inventoryManager.rightHandWeapon == null || states.inventoryManager.rightHandWeapon.instance == null)
            {
                Debug.LogWarning("ActionManager: right-hand weapon is missing, skip one-handed action refresh.");
                return;
            }

            EmptyAllSlot();

            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.f, ActionInput.f, actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.r, ActionInput.r, actionSlots);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.f, ActionInput.e, actionSlots, true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.r, ActionInput.q, actionSlots, true);
            }
            else
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.e, ActionInput.e, actionSlots);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.q, ActionInput.q, actionSlots);
            }
        }
        public void UpdateActionsTwoHanded()
        {
            if (states == null || states.inventoryManager == null)
            {
                Debug.LogWarning("ActionManager: inventoryManager is not ready, skip two-handed action refresh.");
                return;
            }

            EmptyAllSlot();
            if (states.inventoryManager.rightHandWeapon == null)
            {
                Debug.LogWarning("ActionManager: rightHandWeapon is null in two-handed refresh.");
                return;
            }

            Weapon w = states.inventoryManager.rightHandWeapon.instance;

            if (w == null)
                return;

            if (w.two_handedActions == null || w.two_handedActions.Count == 0)
            {
                Debug.LogWarning("Two-handed mode enabled but weapon has no two_handedActions.");
                return;
            }

            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action source = w.two_handedActions[i];
                Action a = StaticFunctions.GetAction(source.input, actionSlots);
                if (a != null)
                {
                    a.targetAnim = source.targetAnim;
                    a.actionType = source.actionType;
                    a.spellClass = source.spellClass;
                    a.mirror = source.mirror;
                    a.canBeParried = source.canBeParried;
                    a.changeSpeed = source.changeSpeed;
                    a.animSpeed = source.animSpeed;
                    a.canParry = source.canParry;
                    a.canBackStab = source.canBackStab;
                    a.staminaCost = source.staminaCost;
                    a.focusCost = source.focusCost;
                    a.ovverideDamageAnim = source.ovverideDamageAnim;
                    a.damageAnim = source.damageAnim;
                    a.parryMultiplier = source.parryMultiplier;
                    a.backstabMultiplier = source.backstabMultiplier;
                    if (source.weaponStats != null && a.weaponStats != null)
                        StaticFunctions.DeepCopyWeaponStats(source.weaponStats, a.weaponStats);
                }
            }

            // Keep Q/E usable in two-handed mode by mirroring right-hand
            // slots when left-hand slots are not explicitly configured.
            Action a_f = StaticFunctions.GetAction(ActionInput.f, actionSlots);
            Action a_r = StaticFunctions.GetAction(ActionInput.r, actionSlots);
            Action a_e = StaticFunctions.GetAction(ActionInput.e, actionSlots);
            Action a_q = StaticFunctions.GetAction(ActionInput.q, actionSlots);

            if (a_e != null && string.IsNullOrEmpty(a_e.targetAnim) && a_f != null && !string.IsNullOrEmpty(a_f.targetAnim))
                StaticFunctions.DeepCopyActionToAction(a_e, a_f);
            if (a_q != null && string.IsNullOrEmpty(a_q.targetAnim) && a_r != null && !string.IsNullOrEmpty(a_r.targetAnim))
                StaticFunctions.DeepCopyActionToAction(a_q, a_r);
        }




        void EmptyAllSlot()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);
                a.targetAnim = null;
                a.mirror = false;
                a.actionType = ActionType.attack;

            }
        }
        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }

        }
        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return StaticFunctions.GetAction(a_input, actionSlots);
        }
        public ActionInput GetActionInput(StateManager st)
        {
            if (st.f)
                return ActionInput.f;
            if (st.r)
                return ActionInput.r;
            if (st.e)
                return ActionInput.e;
            if (st.q)
                return ActionInput.q;

            return ActionInput.f;
        }
        public bool IsLeftHandslot(Action slot)
        {
            return (slot.input == ActionInput.e || slot.input == ActionInput.q);
        }
    }
    public enum ActionInput
    {
        f, e, r, q,
    }

    public enum ActionType
    {
        attack, block, spells, parry
    }
    public enum SpellClass
    {
        pyromancy, miracles, sorcery
    }
    public enum SpellType
    {
        projectile, buff, looping
    }
    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public ActionType actionType;
        public SpellClass spellClass;
        public string targetAnim;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackStab = false;
        public float staminaCost = 5;
        public int focusCost = 0;

        [HideInInspector]
        public float parryMultiplier;

        [HideInInspector]
        public float backstabMultiplier;

        public bool ovverideDamageAnim;
        public string damageAnim;

        public WeaponStats weaponStats;

    }
    [System.Serializable]
    public class SpellAction
    {
        public ActionInput input;
        public string targetAnim;
        public string throwAnim;
        public float castTime;
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnim;
        public string item_id;
    }
}


