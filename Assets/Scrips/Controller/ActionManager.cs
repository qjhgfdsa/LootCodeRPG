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
            EmptyAllSlot();

            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlots);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, actionSlots, true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, actionSlots, true);
            }
            else
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lb, ActionInput.lb, actionSlots);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lt, ActionInput.lt, actionSlots);
            }
        }
        /*   public void UpdateActionsWithLeftHand()
           {
               Weapon r_w = states.inventoryManager.rightHandWeapon;
               Weapon l_w = states.inventoryManager.leftHandWeapon;

               Action rb = GetAction(ActionInput.rb);
               Action rt = GetAction(ActionInput.rt);
               rb.targetAnim = r_w.GetAction(r_w.actions, ActionInput.rb).targetAnim;
               rt.targetAnim = r_w.GetAction(r_w.actions, ActionInput.rt).targetAnim;

               Action lb = GetAction(ActionInput.rb);
               Action lt = GetAction(ActionInput.lt);
               lb.targetAnim = l_w.GetAction(l_w.actions, ActionInput.rb).targetAnim;
               lt.targetAnim = l_w.GetAction(l_w.actions, ActionInput.rt).targetAnim;

           }*/

        /*  public void UpdateActionsTwoHanded()
         {
             EmptyAllSlot();
             Weapon w = states.inventoryManager.rightHandWeapon;

             for (int i = 0; i < w.two_handedActions.Count; i++)
             {
                 Action a = GetAction(w.actions[i].input);
                 a.targetAnim = w.two_handedActions[i].targetAnim;
             }
         }*/
     /*   public void DeepCopyAction(Weapon w, ActionInput inp, ActionInput assing, bool isLeftHand = false)
        {
            Action a = GetAction(assing);
            Action w_a = w.GetAction(w.actions, inp);
            if (w_a == null)
                return;
            a.targetAnim = w_a.targetAnim;
            a.actionType = w_a.actionType;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackStab = w_a.canBackStab;
            a.ovverideDamageAnim = w_a.ovverideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;


            if (isLeftHand)
            {
                a.mirror = true;
            }

            DeepCopyWeaponStats(w_a.weaponStats, a.weaponStats);
        }

        public void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {
            to.physical = from.physical;
            to.slash = from.slash;
            to.strike = from.strike;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.lightning = from.lightning;
            to.fire = from.fire;
            to.dark = from.dark;
        } */

        public void UpdateActionsTwoHanded()
        {

            EmptyAllSlot();
            Weapon w = states.inventoryManager.rightHandWeapon.instance;

            if (w == null || w.two_handedActions == null) return;

            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = StaticFunctions.GetAction(w.two_handedActions[i].input, actionSlots); // แก้ไขตรงนี้
                if (a != null)
                    a.targetAnim = w.two_handedActions[i].targetAnim;
                a.actionType = w.two_handedActions[i].actionType;
            }
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

            if (st.rb)
                return ActionInput.rb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lb)
                return ActionInput.lb;
            if (st.lt)
                return ActionInput.lt;

            return ActionInput.rb;
        }

        public bool IsLeftHandslot(Action slot)
        {
            return (slot.input == ActionInput.lb || slot.input == ActionInput.lt);
        }
    }


    public enum ActionInput
    {
        rb, lb, rt, lt,
    }

    public enum ActionType
    {
        attack, block, spells, parry
    }
     public enum SpellType
    {
        pyromancy, miracles, sorcery
    }



    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public ActionType actionType;
        public SpellType spellType;
        public string targetAnim;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackStab = false;

        [HideInInspector]
        public float parryMultiplier;

        [HideInInspector]
        public float backstabMultiplier;

        public bool ovverideDamageAnim;
        public string damageAnim;

        public WeaponStats weaponStats;

    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnim;
        public string item_id;
    }
}


