using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;



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

            DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.rb, ActionInput.rb);
            DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.rt, ActionInput.rt);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.rb, ActionInput.lb, true);
                DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.rt, ActionInput.lt, true);
            }
            else
            {
                DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.lb, ActionInput.lb);
                DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.lt, ActionInput.lt);
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
        public void DeepCopyAction(Weapon w, ActionInput inp, ActionInput assing, bool isLeftHand = false)
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

            if(isLeftHand)
            {      
                a.mirror = true;
            }
        }

        public void UpdateActionsTwoHanded()
        {

            EmptyAllSlot();
            Weapon w = states.inventoryManager.rightHandWeapon;

            if (w == null || w.two_handedActions == null) return;

            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = GetAction(w.two_handedActions[i].input); // แก้ไขตรงนี้
                if (a != null)
                    a.targetAnim = w.two_handedActions[i].targetAnim;
                    a.actionType = w.two_handedActions[i].actionType;
            }
        }


     

         void EmptyAllSlot()
         {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
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
             return GetAction(a_input);

         }

         Action GetAction(ActionInput input)
         {

             for (int i = 0; i < actionSlots.Count; i++)
             {
                 if (actionSlots[i].input == input)
                     return actionSlots[i];
             }

             return null;

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
        attack,block,spells,parry
    }


    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public ActionType actionType;
        public string targetAnim;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canBackStab = false;
     }

     [System.Serializable]
     public class ItemAction
     {
         public string targetAnim;
         public string item_id;
     }
}


