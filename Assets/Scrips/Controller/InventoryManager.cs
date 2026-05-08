using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class InventoryManager : MonoBehaviour
    {
        public List<string> rh_weapons;
        public List<string> lh_weapons;
        public List<string> spell_items;

        public int r_index;
        public int l_index;
        public int s_index;
        List<RuntimeWeapon> r_r_weapons = new List<RuntimeWeapon>();
        List<RuntimeWeapon> r_l_weapons = new List<RuntimeWeapon>();
        List<RuntimeSpellItems> r_spells = new List<RuntimeSpellItems>();

        public RuntimeSpellItems currentSpell;
        public RuntimeWeapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public RuntimeWeapon leftHandWeapon;

        public GameObject parryCollider;
        public GameObject breathCollider;
        public GameObject blockCollider;

        StateManager states;

        public void Init(StateManager st)
        {

            states = st;
            LoadInventory();


            // InitAllDamageCollider(st);
            // CloseAllDamageColliders();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
            CloseBreathCollider();
            CloseBlockCollider();
        }

        public void LoadInventory()
        {
            for (int i = 0; i < rh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(rh_weapons[i]));
            }

            for (int i = 0; i < lh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(lh_weapons[i]), true);
            }

            if (r_r_weapons.Count > 0)
            {
                if (r_index > r_r_weapons.Count - 1)
                    r_index = 0;

                rightHandWeapon = r_r_weapons[r_index];
            }

            if (r_l_weapons.Count > 0)
            {
                if (l_index > r_l_weapons.Count - 1)
                    l_index = 0;

                leftHandWeapon = r_l_weapons[l_index];
            }

            if (rightHandWeapon != null && rightHandWeapon.weaponModel != null)
                EquipWeapon(rightHandWeapon, false);

            if (leftHandWeapon != null && leftHandWeapon.weaponModel != null)
            {
                EquipWeapon(leftHandWeapon, true);
                hasLeftHandWeapon = true;
            }

            for (int i = 0; i < spell_items.Count; i++)
            {
                SpellToRuntimeSpell(ResourcesManager.singleton.GetSpell(spell_items[i]));
            }

            hasLeftHandWeapon = (leftHandWeapon != null);

            if (r_spells.Count > 0)
            {
                if (s_index > r_spells.Count - 1)
                    s_index = 0;

                EquipSpell(r_spells[s_index]);
            }

            InitAllDamageCollider(states);
            CloseAllDamageColliders();


        }
        public RuntimeSpellItems SpellToRuntimeSpell(Spell s, bool isLeft = false)
        {
            if (s == null)
            {
                Debug.LogWarning("InventoryManager: Spell asset is null, skip adding runtime spell.");
                return null;
            }

            GameObject go = new GameObject();
            RuntimeSpellItems inst = go.AddComponent<RuntimeSpellItems>();
            inst.instance = new Spell();
            StaticFunctions.DeepCopySpell(s, inst.instance);
            go.name = s.itemName;



            r_spells.Add(inst);
            return inst;
        }

        public void CreateSpellParticle(RuntimeSpellItems inst, bool isLeft, bool parentUnderRoot = false)
        {
            if (inst == null || inst.instance == null)
            {
                Debug.LogError("CreateSpellParticle: inst or inst.instance is null");
                return;
            }
            if (inst.instance.particlePrefab == null)
            {
                Debug.LogWarning($"CreateSpellParticle: particlePrefab is null for spell '{inst.instance.itemName}'");
                return;
            }

            if (inst.currentParticle == null)
                inst.currentParticle = Instantiate(inst.instance.particlePrefab) as GameObject;

            // Rebind every call in case currentParticle was created earlier but p_hook was never assigned.
            // Include inactive children because some FX prefabs keep hook objects disabled.
            inst.p_hook = inst.currentParticle.GetComponentInChildren<ParticleHook>(true);
            if (inst.p_hook == null)
            {
                Debug.LogError($"CreateSpellParticle: ParticleHook not found on '{inst.currentParticle.name}'");
                return;
            }
            inst.p_hook.Init();

            if (!parentUnderRoot)
            {
                Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
                inst.currentParticle.transform.parent = p;
                inst.currentParticle.transform.localRotation = Quaternion.identity;
                inst.currentParticle.transform.localPosition = Vector3.zero;
            }
            else
            {
                inst.currentParticle.transform.parent = transform;
                // firebreath2 emits sideways in local space, so compensate by -90 yaw.
                inst.currentParticle.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                inst.currentParticle.transform.localPosition = new Vector3(0, 1.5f, 0.4f);
            }

        }
        public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w, bool isLeft = false)
        {

            GameObject go = new GameObject();
            RuntimeWeapon inst = go.AddComponent<RuntimeWeapon>();
            go.name = w.itemName;

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

            if (isLeft)
            {
                r_l_weapons.Add(inst);
            }
            else
            {
                r_r_weapons.Add(inst);
            }

            inst.weaponModel.SetActive(false);
            return inst;

        }


        public void EquipWeapon(RuntimeWeapon w, bool isLeft = false)
        {
            if (isLeft)
            {
                if (leftHandWeapon != null)
                {
                    leftHandWeapon.weaponModel.SetActive(false);
                }
                leftHandWeapon = w;
            }
            else
            {
                if (rightHandWeapon != null)
                {
                    rightHandWeapon.weaponModel.SetActive(false);
                }
                rightHandWeapon = w;
            }
            String targetIdle = w.instance.oh_idle;
            targetIdle += isLeft ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            if (UI.QuickSlot.singleton != null)
            {
                UI.QuickSlot.singleton.UpdateSlot(
                    (isLeft) ?
                    UI.QSlotType.lh : UI.QSlotType.rh, w.instance.icon);
            }
            else
                Debug.LogWarning("InventoryManager: QuickSlot.singleton is null — add a QuickSlot to the scene.");

            w.weaponModel.SetActive(true);
        }

        public void EquipSpell(RuntimeSpellItems spell)
        {
            currentSpell = spell;
            if (UI.QuickSlot.singleton == null)
            {
                Debug.LogWarning("InventoryManager: QuickSlot.singleton is null — add a QuickSlot to the scene.");
                return;
            }

            UI.QuickSlot.singleton.UpdateSlot(UI.QSlotType.spell, spell.instance.icon);
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
        public void ChangeToNextWeapon(bool isLeft)
        {
            if (isLeft)
            {
                if (l_index < r_l_weapons.Count - 1)
                    l_index++;
                else
                    l_index = 0;

                EquipWeapon(r_l_weapons[l_index], true);
            }
            else
            {
                if (r_index < r_r_weapons.Count - 1)
                    r_index++;
                else
                    r_index = 0;

                EquipWeapon(r_r_weapons[r_index]);
            }

            states.actionManager.UpdateActionsOneHanded();
        }

        public void ChangeToNextSpell()
        {
            if (r_spells.Count == 0)
            {
                Debug.LogWarning("InventoryManager: No runtime spells to cycle. Check spell_items and ResourcesManager spell ids.");
                return;
            }

            if (s_index < r_spells.Count - 1)
                s_index++;
            else
                s_index = 0;

            EquipSpell(r_spells[s_index]);
        }

        #region Delegate Calls
        public void OpenBreathCollider()
        {
            breathCollider.SetActive(true);
        }
        public void CloseBreathCollider()
        {
            breathCollider.SetActive(false);
        }
        public void OpenBlockCollider()
        {
            blockCollider.SetActive(true);
        }
        public void CloseBlockCollider()
        {
            blockCollider.SetActive(false);
        }
        public void EmitSpellParticle()
        {
            currentSpell.p_hook.Emit(1);
        }
        #endregion
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
            if (l == null)
            {
                Debug.Log("List of actions is null");
                return null;
            }

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
        public SpellClass spellClass;
        public List<SpellAction> actions = new List<SpellAction>();
        public GameObject projecttile;
        public GameObject particlePrefab;
        public string spell_effect;
        public SpellAction GetAction(List<SpellAction> l, ActionInput inp)
        {
            if (l == null)
            {
                Debug.Log("List of actions is null");
                return null;
            }

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

