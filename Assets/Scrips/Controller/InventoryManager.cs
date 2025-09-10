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
        public void Init()
        {
            CloseAllDamageColliders();
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
