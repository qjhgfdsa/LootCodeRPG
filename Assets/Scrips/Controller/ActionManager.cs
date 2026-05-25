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
                    a.fristStep = new ActionAnim();
                    a.fristStep.input = (ActionInput)i;
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
                actionSlots[i].fristStep.input = (ActionInput)i;
        }
        public void UpdateActionsTwoHanded()
        {

            EmptyAllSlot();

            Weapon w = states.inventoryManager.rightHandWeapon.instance;

            if (w == null)
                return;

            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = StaticFunctions.GetAction(w.two_handedActions[i].GetfirstInput(), actionSlots);

                StaticFunctions.DeepCopyStepList(w.two_handedActions[i], a);
                a.actionType = w.two_handedActions[i].actionType;
            }


        }

        void EmptyAllSlot()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);

                if (a == null)
                    return;
                    
                a.fristStep = null;
                a.comboSteps = null;
                a.mirror = false;
                a.actionType = ActionType.attack;
            }
        }

        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                //  a.input = (ActionInput)i;
                actionSlots.Add(a);
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
            return (slot.GetfirstInput() == ActionInput.e || slot.GetfirstInput() == ActionInput.q);
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
        public ActionType actionType;
        public SpellClass spellClass;

        public ActionAnim fristStep;
        public List<ActionAnim> comboSteps;

        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackStab = false;
        public float staminaCost = 5;
        public int focusCost = 0;

        public ActionInput GetfirstInput()
        {
            if (fristStep == null)
                fristStep = new ActionAnim();

            return fristStep.input;
        }

        public ActionAnim GetActionStep(ref int indx)
        {
            if (comboSteps.Count == 0)
                return fristStep;

            if (indx > comboSteps.Count - 1)
                indx = 0;

            ActionAnim retVal = comboSteps[indx];

            if (indx < comboSteps.Count - 1)
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


