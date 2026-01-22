using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


namespace SA
{
    public class InventoryManager : MonoBehaviour
    {
        public Weapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public Weapon leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            EquipWeapon(rightHandWeapon, false);
            EquipWeapon(leftHandWeapon, true);

            CloseAllDamageColliders();
            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
        }

        public void EquipWeapon(Weapon w, bool isLeft = false)
        {
            String targetIdle = w.oh_idle;
            targetIdle += isLeft ? "_l" : "_r";
            states.anim.SetBool("mirror", isLeft);
            states.anim.Play("changeWeapon");
            states.anim.Play(targetIdle);

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot(
                (isLeft) ?
                UI.QSlotType.lh : UI.QSlotType.rh, w.icon);

          
        }
        public void OpenAllDamageColliders()
        {
            if (rightHandWeapon.w_Hook != null)
                rightHandWeapon.w_Hook.OpenDamageColliders();


            if (leftHandWeapon.w_Hook != null)
                leftHandWeapon.w_Hook.OpenDamageColliders();

        }



        public void CloseAllDamageColliders()
        {

            if (rightHandWeapon.w_Hook != null)
                rightHandWeapon.w_Hook.CloseDamageColliders();


            if (leftHandWeapon.w_Hook != null)
                leftHandWeapon.w_Hook.CloseDamageColliders();
        }

        public void CloseParryCollider()
        {
            parryCollider.SetActive(false);
        }

         public void OpenParryCollider()
        {
            parryCollider.SetActive(true);
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public string weaponId;
        public Sprite icon;
        public string oh_idle;
        public string th_idle;
        public List<Action> actions;
        public List<Action> two_handedActions;
        public bool LeftHandMirror;
        public GameObject weaponModel;
        public WeaponHook w_Hook;

        public Action GetAction(List<Action> l , ActionInput inp)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                {
                    return l[i];
                }
            }

            return null;
        }
    }
}
