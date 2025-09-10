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

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            EquipRightWeapon();
            EquipLeftWeapon();
            CloseAllDamageColliders();
        }

        public void EquipRightWeapon()
        {
            string targetIdle = rightHandWeapon.oh_idle;
            targetIdle += "_r";

            states.anim.SetBool("mirror", false);
            states.anim.Play("changeWeapon");
            states.anim.Play(targetIdle);
        }
        public void EquipLeftWeapon()
        {
            if (hasLeftHandWeapon == false)
                return;

            string targetIdle = leftHandWeapon.oh_idle;
            targetIdle += "_l";

            states.anim.SetBool("mirror", true);
            states.anim.Play("changeWeapon");
            states.anim.Play(targetIdle);
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
    }

    [System.Serializable]
    public class Weapon
    {
        public string oh_idle;
        public string th_idle;
        public List<Action> actions;
        public List<Action> two_handedActions;
        public bool LeftHandmirror;
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
