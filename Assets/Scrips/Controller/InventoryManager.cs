using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class InventoryManager : MonoBehaviour
    {
        public string unarmedId = "Unarmed";
        public RuntimeWeapon unarmedRuntime;

        public List<string> rh_weapons;
        public List<string> lh_weapons;
        public List<string> spell_items;
        public List<string> consumable_items;

        public int r_index;
        public int l_index;
        public int s_index;
        public int c_index;

        List<RuntimeWeapon> r_r_weapons = new List<RuntimeWeapon>();
        List<RuntimeWeapon> r_l_weapons = new List<RuntimeWeapon>();
        List<RuntimeSpellItems> r_spells = new List<RuntimeSpellItems>();
        List<RuntimeConsumable> r_consum = new List<RuntimeConsumable>();

        public RuntimeConsumable currentConsumable;
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

            unarmedRuntime = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(unarmedId), false);
            if (unarmedRuntime == null)
            {
                Debug.LogError($"InventoryManager: Could not load unarmed weapon id '{unarmedId}'. itemName in WeaponScriptableObject must match exactly (case-sensitive).");
                return;
            }

            for (int i = 0; i < rh_weapons.Count; i++)
            {
                RuntimeWeapon rw = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(rh_weapons[i]));
                r_r_weapons.Add(rw);
            }

            for (int i = 0; i < lh_weapons.Count; i++)
            {
                RuntimeWeapon lw = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(lh_weapons[i]), true);
                r_l_weapons.Add(lw);
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

            for (int i = 0; i < consumable_items.Count; i++)
            {
                RuntimeConsumable c = ConsumableToRuntime(ResourcesManager.singleton.GetConsumable(consumable_items[i]));
                r_consum.Add(c);
            }

            if (r_consum.Count > 0)
            {
                if (c_index > r_consum.Count - 1)
                    c_index = 0;

                EquipConsumable(r_consum[c_index]);
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

            inst.weaponStats = new WeaponStats();
            WeaponStats w_stats = ResourcesManager.singleton.GetWeaponStats(w.itemName);
            StaticFunctions.DeepCopyWeaponStats(w_stats, inst.weaponStats);

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

            inst.weaponModel.SetActive(false);
            return inst;

        }

        public RuntimeConsumable ConsumableToRuntime(Consumable c)
        {
            GameObject go = new GameObject();
            RuntimeConsumable inst = go.AddComponent<RuntimeConsumable>();
            go.name = c.itemName;

            inst.instance = new Consumable();
            StaticFunctions.DeepCopyConsumable(c, inst.instance);


            if (inst.instance.itemPrefab != null)
            {
                GameObject model = Instantiate(inst.instance.itemPrefab) as GameObject;
                Transform p = states.anim.GetBoneTransform(HumanBodyBones.RightHand);
                model.transform.parent = p;
                model.transform.localPosition = inst.instance.r_model_pos;
                model.transform.localEulerAngles = inst.instance.r_model_eulers;


                Vector3 targetScale = inst.instance.model_scale;
                if (targetScale == Vector3.zero)
                    targetScale = Vector3.one;
                model.transform.localScale = targetScale;

                inst.itemModel = model;
                inst.itemModel.SetActive(false);
            }

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
                    UI.QSlotType.lh : UI.QSlotType.rh, w.instance.GetIconId());
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

            UI.QuickSlot.singleton.UpdateSlot(UI.QSlotType.spell, spell.instance.GetIconId());
        }
        public void EquipConsumable(RuntimeConsumable consum)
        {
            currentConsumable = consum;
            if (UI.QuickSlot.singleton == null)
            {
                Debug.LogWarning("InventoryManager: QuickSlot.singleton is null — add a QuickSlot to the scene.");
                return;
            }

            UI.QuickSlot.singleton.UpdateSlot(UI.QSlotType.item, consum.instance.GetIconId());

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
            if (rightHandWeapon != null && rightHandWeapon.w_Hook != null)
                rightHandWeapon.w_Hook.OpenDamageColliders();

            if (leftHandWeapon != null && leftHandWeapon.w_Hook != null)
                leftHandWeapon.w_Hook.OpenDamageColliders();
        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapon != null && rightHandWeapon.w_Hook != null)
                rightHandWeapon.w_Hook.CloseDamageColliders();

            if (leftHandWeapon != null && leftHandWeapon.w_Hook != null)
                leftHandWeapon.w_Hook.CloseDamageColliders();
        }

        public void InitAllDamageCollider(StateManager state)
        {
            if (rightHandWeapon != null && rightHandWeapon.w_Hook != null)
                rightHandWeapon.w_Hook.InitDamageCollider(state);

            if (leftHandWeapon != null && leftHandWeapon.w_Hook != null)
                leftHandWeapon.w_Hook.InitDamageCollider(state);
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
            states.isTwoHanded = false;
            states.HandleTwoHanded();

            if (isLeft)
            {
                if (r_l_weapons.Count == 0)
                    return;

                if (l_index < r_l_weapons.Count - 1)
                    l_index++;
                else
                    l_index = 0;

                EquipWeapon(r_l_weapons[l_index], true);
            }
            else
            {
                if (r_r_weapons.Count == 0)
                    return;

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
        public void ChangeToNextConsumable()
        {
            if (r_consum.Count == 0)
            {
                Debug.LogWarning("InventoryManager: No runtime consumables to cycle. Check consumable_items and ResourcesManager consumable ids.");
                return;
            }

            if (c_index < r_consum.Count - 1)
                c_index++;
            else
                c_index = 0;

            EquipConsumable(r_consum[c_index]);
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
        [Tooltip("Registry key. Leave empty to use normalized itemName.")]
        public string iconId;
        public Sprite icon;

        public IconId GetIconId()
        {
            if (!string.IsNullOrWhiteSpace(iconId))
                return new IconId(IconId.Normalize(iconId));

            return IconId.FromItemName(itemName);
        }
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
                if (l[i].GetfirstInput() == inp)
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
    [System.Serializable]
    public class Consumable : Item
    {
        public string consumableEffect;
        public string targetAnim;

        public GameObject itemPrefab;
        public Vector3 r_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 model_scale;
    }

    #region IconId
    [Serializable]
    public readonly struct IconId : IEquatable<IconId>
    {
        public readonly string Value;

        public IconId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        public bool IsEmpty => string.IsNullOrEmpty(Value);

        public static IconId FromItemName(string itemName)
        {
            return new IconId(Normalize(itemName));
        }

        public static string Normalize(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            return raw.Trim().ToLowerInvariant().Replace(' ', '_');
        }

        public bool Equals(IconId other) => Value == other.Value;

        public override bool Equals(object obj) => obj is IconId other && Equals(other);

        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;

        public override string ToString() => Value ?? string.Empty;

        public static bool operator ==(IconId a, IconId b) => a.Equals(b);

        public static bool operator !=(IconId a, IconId b) => !a.Equals(b);
        #endregion
    }
}

