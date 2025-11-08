using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                UpdateActionsWithLeftHand();
                return;
            }
            Weapon w = states.inventoryManager.rightHandWeapon;

            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetAction(w.actions[i].input);
                a.targetAnim = w.actions[i].targetAnim;
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
        public void DeepCopyAction()
        {
            
        }
        public void UpdateActionsWithLeftHand()
        {

            Weapon r_w = states.inventoryManager.rightHandWeapon;
            Weapon l_w = states.inventoryManager.leftHandWeapon;

            // ตรวจสอบว่า weapon ไม่เป็น null
            if (r_w == null || l_w == null) return;

            // Right hand actions
            Action rb = GetAction(ActionInput.rb);
            Action rt = GetAction(ActionInput.rt);

         

            var rightRB = r_w.GetAction(r_w.actions, ActionInput.rb);
            var rightRT = r_w.GetAction(r_w.actions, ActionInput.rt);

            Action w_rb = r_w.GetAction(r_w.actions, ActionInput.rb);
            if (rightRB != null) rb.targetAnim = w_rb.targetAnim;
            rb.actionType = w_rb.actionType;
            rb.canBeParried = w_rb.canBeParried;
            rb.changeSpeed = w_rb.changeSpeed;
            rb.animSpeed = w_rb.animSpeed;
              
            Action w_rt = r_w.GetAction(r_w.actions, ActionInput.rt);
            if (rightRT != null) rt.targetAnim = w_rt.targetAnim;
            rt.actionType = w_rt.actionType;
            rt.canBeParried = w_rt.canBeParried;
            rt.changeSpeed = w_rt.changeSpeed;
            rt.animSpeed = w_rt.animSpeed;
          
            // Left hand actions - แก้ไขตรงนี้
            Action lb = GetAction(ActionInput.lb);
            Action lt = GetAction(ActionInput.lt);

          

            var leftLB = l_w.GetAction(l_w.actions, ActionInput.lb); // เปลี่ยนจาก rb เป็น lb
            var leftLT = l_w.GetAction(l_w.actions, ActionInput.lt); // เปลี่ยนจาก rt เป็น lt

            Action w_lb = l_w.GetAction(l_w.actions, ActionInput.lb);
            if (leftLB != null) lb.targetAnim = w_lb.targetAnim;
            lb.actionType = w_lb.actionType;
            lb.canBeParried = w_lb.canBeParried;
            lb.changeSpeed = w_lb.changeSpeed;
            lb.animSpeed = w_lb.animSpeed;  

            Action w_lt = l_w.GetAction(l_w.actions, ActionInput.lt);
            if (leftLT != null) lt.targetAnim = w_lt.targetAnim;
            lt.actionType = w_lt.actionType;
            lt.canBeParried = w_lt.canBeParried;
            lt.changeSpeed = w_lt.changeSpeed;
            lt.animSpeed = w_lt.animSpeed;
            
            if (l_w.LeftHandMirror)
            {
                lb.mirror = true;
                lt.mirror = true;
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
     }

     [System.Serializable]
     public class ItemAction
     {
         public string targetAnim;
         public string item_id;
     }
}


