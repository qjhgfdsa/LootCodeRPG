using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class InventoryManager : MonoBehaviour
    {
        public List<string> rh_weapons;
        public List<string> lh_weapons;

        public ItemInstance rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public ItemInstance leftHandWeapon;

        public GameObject parryCollider;

        StateManager states;

        /*  public void Init(StateManager st)
          {

              states = st;

              // เช็คว่า states พร้อมหรือยัง
              if (states == null || states.anim == null)
              {
                  Debug.LogError("❌ StateManager หรือ Animator ยังไม่พร้อม!");
                  return;
              }

              rightHandWeapon = WeaponToItemInstance(ResourcesManager.singleton.GetWeapon(rh_weapons[0]));

              if (rightHandWeapon != null)
                  EquipWeapon(rightHandWeapon, false);

              if (leftHandWeapon != null)
                  EquipWeapon(leftHandWeapon, true);

              hasLeftHandWeapon = (leftHandWeapon != null);


              InitAllDamageCollider(st);
              CloseAllDamageColliders();

              ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
              pr.InitPlayer(st);
              CloseParryCollider();
          }

          public ItemInstance WeaponToItemInstance(Weapon w, bool isLeft = false)
          {
              // เช็คก่อนใช้
              if (states == null)
              {
                  Debug.LogError("❌ states is NULL in WeaponToItemInstance!");
                  return null;
              }

              if (states.anim == null)
              {
                  Debug.LogError("❌ states.anim is NULL in WeaponToItemInstance!");
                  return null;
              }


              GameObject go = new GameObject();
              ItemInstance inst = go.AddComponent<ItemInstance>();

              inst.instance = new Weapon();
              StaticFunctions.DeepCopyWeapon(w, inst.instance);

              inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
              Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
              inst.weaponModel.transform.parent = p;
              inst.weaponModel.transform.localPosition = inst.instance.model_pos;
              inst.weaponModel.transform.localEulerAngles = inst.instance.model_eulers;
              inst.weaponModel.transform.localScale = inst.instance.model_scale;

              inst.w_Hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
              inst.w_Hook.InitDamageCollider(states);

              return inst;

          } */

        public void Init(StateManager st)
        {
            states = st;

            // เช็คว่า states พร้อมหรือยัง
            if (states == null || states.anim == null)
            {
                Debug.LogError("❌ StateManager หรือ Animator ยังไม่พร้อม!");
                return;
            }

            // เช็คว่ามี weapon list หรือไม่
            if (rh_weapons == null || rh_weapons.Count == 0)
            {
                Debug.LogError("❌ rh_weapons list ว่างเปล่า!");
                return;
            }

            // ดึง weapon
            Weapon rightWeapon = ResourcesManager.singleton.GetWeapon(rh_weapons[0]);

            if (rightWeapon == null)
            {
                Debug.LogError("❌ ไม่เจอ weapon: " + rh_weapons[0]);
                return;
            }

            rightHandWeapon = WeaponToItemInstance(rightWeapon);

            // เช็คก่อน Equip
            if (rightHandWeapon != null && rightHandWeapon.weaponModel != null)
            {
                EquipWeapon(rightHandWeapon, false);
            }
            else
            {
                Debug.LogError("❌ rightHandWeapon หรือ weaponModel เป็น null!");
            }

            if (leftHandWeapon != null && leftHandWeapon.weaponModel != null)
            {
                EquipWeapon(leftHandWeapon, true);
            }

            hasLeftHandWeapon = (leftHandWeapon != null);

            InitAllDamageCollider(st);
            CloseAllDamageColliders();

            if (parryCollider != null)
            {
                ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
                if (pr != null)
                    pr.InitPlayer(st);
                CloseParryCollider();
            }
        }

        public ItemInstance WeaponToItemInstance(Weapon w, bool isLeft = false)
        {
            // เช็คทุกอย่างก่อนใช้
            if (w == null)
            {
                Debug.LogError("❌ Weapon is NULL!");
                return null;
            }

            if (states == null)
            {
                Debug.LogError("❌ states is NULL!");
                return null;
            }

            if (states.anim == null)
            {
                Debug.LogError("❌ states.anim is NULL!");
                return null;
            }

            if (w.modelPrefab == null)
            {
                Debug.LogError("❌ Weapon modelPrefab is NULL for weapon: " + w.weaponId);
                return null;
            }

            GameObject go = new GameObject();
            ItemInstance inst = go.AddComponent<ItemInstance>();

            inst.instance = new Weapon();
            StaticFunctions.DeepCopyWeapon(w, inst.instance);

            inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;

            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);

            if (p == null)
            {
                Debug.LogError("❌ ไม่เจอ Hand bone! ตรวจสอบว่า Animator เป็น Humanoid หรือไม่");
                Destroy(go);
                return null;
            }

            inst.weaponModel.transform.parent = p;
            inst.weaponModel.transform.localPosition = inst.instance.model_pos;
            inst.weaponModel.transform.localEulerAngles = inst.instance.model_eulers;
            inst.weaponModel.transform.localScale = inst.instance.model_scale;

            inst.w_Hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();

            if (inst.w_Hook != null)
                inst.w_Hook.InitDamageCollider(states);

            return inst;
        }

        public void EquipWeapon(ItemInstance w, bool isLeft = false)
        {
            // เช็คก่อนใช้
            if (w == null || w.instance == null || w.weaponModel == null)
            {
                Debug.LogError("❌ ItemInstance, instance หรือ weaponModel เป็น null!");
                return;
            }

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

        public GameObject modelPrefab;

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
