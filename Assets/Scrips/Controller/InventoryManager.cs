using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class InventoryManager : MonoBehaviour
    {
        public List<string> rh_weapons;
        public List<string> lh_weapons;

        public RuntimeWeapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public RuntimeWeapon leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st)
        {

            states = st;

            if (rh_weapons.Count > 0)
            {
                rightHandWeapon = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(rh_weapons[0]));
            }

            if (lh_weapons.Count > 0)
            {
                leftHandWeapon = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(lh_weapons[0]), true);
                hasLeftHandWeapon = true;
            }

            if (rightHandWeapon != null && rightHandWeapon.weaponModel != null)
                EquipWeapon(rightHandWeapon, false);

            if (leftHandWeapon != null && leftHandWeapon.weaponModel != null)
                EquipWeapon(leftHandWeapon, true);

            hasLeftHandWeapon = (leftHandWeapon != null);


            InitAllDamageCollider(st);
            CloseAllDamageColliders();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
        }

        public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w, bool isLeft = false)
        {

            GameObject go = new GameObject();
            RuntimeWeapon inst = go.AddComponent<RuntimeWeapon>();

            inst.instance = new Weapon();
            StaticFunctions.DeepCopyWeapon(w, inst.instance);

            inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.weaponModel.transform.parent = p;


            inst.weaponModel.transform.localPosition =
            (isLeft) ? inst.instance.l_model_pos : inst.instance.r_model_pos;
            inst.weaponModel.transform.localEulerAngles =
            (isLeft) ? inst.instance.l_model_eulers : inst.instance.r_model_eulers;
            inst.weaponModel.transform.localScale = inst.instance.model_scale;

            inst.w_Hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
            inst.w_Hook.InitDamageCollider(states);

            return inst;

        }


        public void EquipWeapon(RuntimeWeapon w, bool isLeft = false)
        {

            String targetIdle = w.instance.oh_idle;
            targetIdle += isLeft ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot(
                (isLeft) ?
                UI.QSlotType.lh : UI.QSlotType.rh, w.instance.icon);

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

        public void InitAllDamageCollider(StateManager state)
        {

            if (rightHandWeapon.w_Hook != null)
                rightHandWeapon.w_Hook.InitDamageCollider(states);


            if (leftHandWeapon.w_Hook != null)
                leftHandWeapon.w_Hook.InitDamageCollider(states);

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
    public class Item
    {
        public string itemName;
        public string itemDescription;
        public Sprite icon;

    }

    [System.Serializable]
    public class Weapon : Item
    {
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handedActions;

        public float parryMultiplier;
        public float backstabMultiplier;
        public bool LeftHandMirror;

        public GameObject modelPrefab;

        public Vector3 r_model_pos;
        public Vector3 l_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 l_model_eulers;
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
   
    [System.Serializable]
    public class Spell : Item
    {
        public SpellType spellType;
    }

   

}

