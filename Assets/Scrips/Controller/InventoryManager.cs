using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


namespace SA
{
    public class InventoryManager : MonoBehaviour
    {
        public ItemInstance rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public ItemInstance leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;

            if (rightHandWeapon != null)
                EquipWeapon(rightHandWeapon.instance, false);

            if (leftHandWeapon != null)
                EquipWeapon(leftHandWeapon.instance, true);

            hasLeftHandWeapon = (leftHandWeapon != null);


            InitAllDamageCollider(st);
            CloseAllDamageColliders();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
        }

        public void EquipWeapon(Weapon w, bool isLeft = false)
        {
            String targetIdle = w.oh_idle;
            targetIdle += isLeft ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot(
                (isLeft) ?
                UI.QSlotType.lh : UI.QSlotType.rh, w.icon);
            
            w.weaponModel.SetActive(true);


        }

        public Weapon GetCurrentWeapon(bool isLeft)
        {
            if (isLeft)
                return leftHandWeapon.instance;
            else
                return rightHandWeapon.instance;
        }
        public void OpenAllDamageColliders()
        {
            if (rightHandWeapon.instance.w_Hook != null)
                rightHandWeapon.instance.w_Hook.OpenDamageColliders();


            if (leftHandWeapon.instance.w_Hook != null)
                leftHandWeapon.instance.w_Hook.OpenDamageColliders();

        }
        public void CloseAllDamageColliders()
        {

            if (rightHandWeapon.instance.w_Hook != null)
                rightHandWeapon.instance.w_Hook.CloseDamageColliders();


            if (leftHandWeapon.instance.w_Hook != null)
                leftHandWeapon.instance.w_Hook.CloseDamageColliders();
        }

        public void InitAllDamageCollider(StateManager state)
        {

            if (rightHandWeapon.instance.w_Hook != null)
                rightHandWeapon.instance.w_Hook.InitDamageCollider(states);


            if (leftHandWeapon.instance.w_Hook != null)
                leftHandWeapon.instance.w_Hook.InitDamageCollider(states);

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

        public float parryMultiplier;
        public float backstabMultiplier;
        public bool LeftHandMirror;

        public GameObject weaponModel;
        public WeaponHook w_Hook;
        
        public Vector3 model_pos;
        public Vector3 model_eulers;
        public Vector3 model_scale;
        
        public Action GetAction(List<Action> l, ActionInput inp)
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
