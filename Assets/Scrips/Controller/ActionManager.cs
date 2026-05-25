using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class ActionManager : MonoBehaviour
    {
        public int actionIndex;
        public List<Action> actionSlots = new List<Action>();
        public ItemAction consumableItem;
        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            if (actionSlots.Count < 4)
            {
                actionSlots.Clear();
                for (int i = 0; i < 4; i++)
                {
                    Action a = new Action();
                    a.input = (ActionInput)i;
                    a.weaponStats = new WeaponStats();
                    actionSlots.Add(a);
                }
            }
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

            if (states.inventoryManager.hasLeftHandWeapon &&
                states.inventoryManager.leftHandWeapon != null &&
                states.inventoryManager.leftHandWeapon.instance != null)
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.f, ActionInput.e, actionSlots, true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.r, ActionInput.q, actionSlots, true);
            }
            else
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.f, ActionInput.e, actionSlots);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.r, ActionInput.q, actionSlots);
            }

            for (int i = 0; i < 4; i++)
                actionSlots[i].input = (ActionInput)i;
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
                int slotIndex = (int)source.input;
                if (slotIndex < 0 || slotIndex >= actionSlots.Count)
                    continue;

                Action a = actionSlots[slotIndex];
                if (a != null)
                {
                    a.steps = source.steps;
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
                    a.overrideDamageAnim = source.overrideDamageAnim;
                    a.damageAnim = source.damageAnim;
                    a.parryMultiplier = source.parryMultiplier;
                    a.backstabMultiplier = source.backstabMultiplier;
                    if (source.weaponStats != null && a.weaponStats != null)
                        StaticFunctions.DeepCopyWeaponStats(source.weaponStats, a.weaponStats);
                }
            }

            // Keep Q/E usable in two-handed mode by mirroring right-hand
            // slots when left-hand slots are not explicitly configured.
            Action a_f = actionSlots[(int)ActionInput.f];
            Action a_r = actionSlots[(int)ActionInput.r];
            Action a_e = actionSlots[(int)ActionInput.e];
            Action a_q = actionSlots[(int)ActionInput.q];

            if (a_e != null && (a_e.steps == null || a_e.steps.Count == 0) && a_f != null && a_f.steps != null && a_f.steps.Count > 0)
            {
                StaticFunctions.DeepCopyActionToAction(a_e, a_f);
                a_e.input = ActionInput.e;
            }
            if (a_q != null && (a_q.steps == null || a_q.steps.Count == 0) && a_r != null && a_r.steps != null && a_r.steps.Count > 0)
            {
                StaticFunctions.DeepCopyActionToAction(a_q, a_r);
                a_q.input = ActionInput.q;
            }
        }




        void EmptyAllSlot()
        {
            while (actionSlots.Count < 4)
            {
                int i = actionSlots.Count;
                Action slot = new Action();
                slot.input = (ActionInput)i;
                slot.weaponStats = new WeaponStats();
                actionSlots.Add(slot);
            }

            for (int i = 0; i < 4; i++)
            {
                if (actionSlots[i] == null)
                {
                    actionSlots[i] = new Action();
                    actionSlots[i].input = (ActionInput)i;
                }

                if (actionSlots[i].weaponStats == null)
                    actionSlots[i].weaponStats = new WeaponStats();

                Action a = actionSlots[i];
                a.steps = null;
                a.mirror = false;
                a.actionType = ActionType.attack;
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            return GetActionFromInput(GetActionInput(st));
        }

        public Action GetActionFromInput(ActionInput a_input)
        {
            int i = (int)a_input;
            if (i < 0 || i >= actionSlots.Count)
                return null;

            return actionSlots[i];
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
            return slot == actionSlots[(int)ActionInput.e] || slot == actionSlots[(int)ActionInput.q];
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
        public List<ActionSteps> steps;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackStab = false;
        public float staminaCost = 5;
        public int focusCost = 0;

        ActionSteps defaultStep;

        public ActionSteps GetActionStep(ref int indx)
        {
            if (steps == null || steps.Count == 0)
            {
                defaultStep = new ActionSteps();
                defaultStep.branches = new List<ActionAnim>();
                ActionAnim aa = new ActionAnim();
                aa.input = input;
                aa.targetAnim = targetAnim;
                defaultStep.branches.Add(aa);
                return defaultStep;
            }


            if (indx > steps.Count - 1)
                indx = 0;

            ActionSteps retVal = steps[indx];

            if (indx < steps.Count - 1)
                indx++;
            else
                indx = 0;

            return retVal;
        }

        [HideInInspector]
        public float parryMultiplier;

        [HideInInspector]
        public float backstabMultiplier;

        public bool overrideDamageAnim;
        public string damageAnim;

        public WeaponStats weaponStats;
    }

    [System.Serializable]
    public class ActionSteps
    {
        public List<ActionAnim> branches = new List<ActionAnim>();

        public ActionAnim GetBranch(ActionInput inp)
        {
            for (int i = 0; i < branches.Count; i++)
            {
                if (branches[i].input == inp)
                {
                    return branches[i];
                }
            }
            return branches[0];
        }
    }

    [System.Serializable]
    public class ActionAnim
    {
        public ActionInput input;
        public string targetAnim;
    }
    [System.Serializable]
    public class SpellAction
    {
        public ActionInput input;
        public string targetAnim;
        public string throwAnim;
        public float castTime;
        public float focusCost;
        public float staminaCost;       
       
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnim;
        public string item_id;
    }
}


