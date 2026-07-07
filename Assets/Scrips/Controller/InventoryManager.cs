using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class InventoryManager : MonoBehaviour
    {
        public string unarmedId;
        public RuntimeWeapon unarmedRuntime;

        [HideInInspector]
        public List<int> rh_weapons;
        [HideInInspector]
        public List<int> lh_weapons;

        public List<string> spell_items;
        public List<int> consumable_items;

        public string m_rh_weapons;
        public string m_lh_weapons;

        public int r_index;
        public int l_index;
        public int s_index;
        public int c_index;

        List<RuntimeWeapon> r_r_weapons = new List<RuntimeWeapon>();
        List<RuntimeWeapon> r_l_weapons = new List<RuntimeWeapon>();
        List<RuntimeSpellItems> r_spells = new List<RuntimeSpellItems>();
        List<RuntimeConsumable> r_consum = new List<RuntimeConsumable>();

        List<RuntimeConsumable> consum_indexes = new List<RuntimeConsumable>();
        List<RuntimeWeapon> lh_indexes = new List<RuntimeWeapon>();
        List<RuntimeWeapon> rh_indexes = new List<RuntimeWeapon>();


        public RuntimeConsumable currentConsumable;
        public RuntimeSpellItems currentSpell;
        public RuntimeWeapon rightHandWeapon;
        public RuntimeWeapon leftHandWeapon;

        RuntimeConsumable emptyItem;

        public GameObject parryCollider;
        public GameObject breathCollider;
        public GameObject blockCollider;

        StateManager states;
        UI.QuickSlot uiSlot;
       public GameObject referenceParent;

        [HideInInspector]
        public ArmorManager armorManager;

        public Vector3 leftHandPos = new Vector3(0.323f, 0.189f, 0.364f);
        public Vector3 leftHandEuler = new Vector3(12.611f, 225.403f, -100.5f);

        public void Init(StateManager st)
        {
            states = st;
            uiSlot = UI.QuickSlot.singleton;

            armorManager = GetComponent<ArmorManager>();

            LoadLists();
            armorManager.Init(states);

            ClearReferences();
            LoadInventory();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
            CloseBreathCollider();
            CloseBlockCollider();
        }

        void LoadLists()
        {

            rh_weapons.Clear();
            lh_weapons.Clear();
            consumable_items.Clear();
            spell_items.Clear();


            for (int i = 0; i < 3; i++)
            {
                rh_weapons.Add(-1);
                lh_weapons.Add(-1);
            }

            for (int i = 0; i < 10; i++)
            {
                consumable_items.Add(-1);
            }

            if (states.isLocal == false)
                return;

            SessionManager s = SessionManager.singleton;

            for (int i = 0; i < s._eq_rh.Count; i++)
            {
                rh_weapons[i] = s._eq_rh[i];
            }

            for (int i = 0; i < s._eq_lh.Count; i++)
            {
                lh_weapons[i] = s._eq_lh[i];
            }

            for (int i = 0; i < s._eq_con.Count; i++)
            {
                consumable_items[i] = s._eq_con[i];
            }
            armorManager.chestId = s._a_chest;
            armorManager.legsId = s._a_legs;
            armorManager.handsId = s._a_hands;
            armorManager.headId = s._a_head;

            spell_items.AddRange(s.spell_Equiped);

        }

        public void ClearReferences()
        {
            if (r_r_weapons != null)
            {
                for (int i = 0; i < r_r_weapons.Count; i++)
                {
                    Destroy(r_r_weapons[i].weaponModel);
                }

                r_r_weapons.Clear();
            }

            if (r_l_weapons != null)
            {
                for (int i = 0; i < r_l_weapons.Count; i++)
                {
                    Destroy(r_l_weapons[i].weaponModel);
                }
                r_l_weapons.Clear();
            }

            leftHandWeapon = null;
            rightHandWeapon = null;

            if (r_consum != null)
            {
                for (int i = 0; i < r_consum.Count; i++)
                {
                    if (r_consum[i] != null && r_consum[i].itemModel)
                        Destroy(r_consum[i].itemModel);
                }
                r_consum.Clear();
            }

            if (r_spells != null)
            {
                for (int i = 0; i < r_spells.Count; i++)
                {
                    if (r_spells[i].currentParticle)
                        Destroy(r_spells[i].currentParticle);
                }
                r_spells.Clear();
            }


            if (referenceParent)
                Destroy(referenceParent);
            referenceParent = new GameObject();
        }
        public void LoadInventory(bool updateActions = false)
        {
            if (!states.isLocal)
            {
                ClearReferences();
            }
            else
            {
                states.sendEquipment = true;
                states.sendWeapons = true;
            }

            SessionManager s = SessionManager.singleton;
            unarmedRuntime = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(unarmedId), false);

            if (unarmedRuntime == null)
            {
                Debug.LogError("InventoryManager: Could not load unarmed weapon");
                return;
            }
            unarmedRuntime.isUnarmed = true;

            for (int i = 0; i < 3; i++)
            {
                r_r_weapons.Add(unarmedRuntime);
                r_l_weapons.Add(unarmedRuntime);

            }

            if (states.isLocal)
            {
                for (int i = 0; i < rh_weapons.Count; i++)
                {
                    if (rh_weapons[i] == -1) //-1 คือ มือเปล่า
                    {
                        r_r_weapons[i] = unarmedRuntime;
                    }
                    else
                    {
                        ItemInventoryInstance it = s.GetWeaponItem(rh_weapons[i]);
                        RuntimeWeapon rw = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(it.itemId));
                        r_r_weapons[i] = rw;
                    }
                }
            }
            else
            {
                if (string.Equals(m_rh_weapons, "มือเปล่า") || string.IsNullOrEmpty(m_rh_weapons))
                {
                    r_r_weapons[0] = unarmedRuntime;
                }
                else
                {
                    RuntimeWeapon rw = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(m_rh_weapons));
                    r_r_weapons[0] = rw;
                }
            }


            if (states.isLocal)
            {
                for (int i = 0; i < lh_weapons.Count; i++)
                {
                    if (states.isLocal)
                    {

                        if (lh_weapons[i] == -1) //-1 คือ มือเปล่า
                        {
                            r_l_weapons[i] = unarmedRuntime;
                        }
                        else
                        {
                            ItemInventoryInstance it = s.GetWeaponItem(lh_weapons[i]);
                            RuntimeWeapon lw = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(it.itemId), true);
                            r_l_weapons[i] = lw;
                        }
                    }
                    else
                    {
                        if (string.Equals(m_lh_weapons, "มือเปล่า") || string.IsNullOrEmpty(m_lh_weapons))
                        {
                            r_l_weapons[0] = unarmedRuntime;
                        }
                        else
                        {
                            RuntimeWeapon lw = WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(m_lh_weapons), true);
                            r_l_weapons[0] = lw;
                        }
                    }
                }
            }

            for (int i = 0; i < spell_items.Count; i++)
            {
                SpellToRuntimeSpell(ResourcesManager.singleton.GetSpell(spell_items[i]));
            }

            Consumable emptyConsumable = ResourcesManager.singleton.GetConsumable("empty");
            if (emptyConsumable == null)
            {
                Debug.LogError("InventoryManager: consumable id 'empty' not found in SA.ConsumablesScriptableObject.");
                return;
            }
            emptyItem = ConsumableToRuntime(emptyConsumable);
            emptyItem.isEmpty = true;

            for (int i = 0; i < 10; i++)
            {
                r_consum.Add(emptyItem);
            }

            for (int i = 0; i < consumable_items.Count; i++)
            {
                if (consumable_items[i] == -1)
                {
                    r_consum[i] = emptyItem;
                }
                else
                {
                    ItemInventoryInstance it = s.GetConItem(consumable_items[i]);

                    RuntimeConsumable c = ConsumableToRuntime(ResourcesManager.singleton.GetConsumable(it.itemId));
                    r_consum[i] = c;
                }
            }

            InitAllDamageCollider(states);
            CloseAllDamageColliders();

            MakeIndexesList();
            EquipInventory();
            armorManager.Init(states);

            if (updateActions)
            {
                if (states.isLocal)
                    states.actionManager.UpdateActionsOneHanded();
            }
        }
        public void EquipInventory()
        {
            if (r_index > rh_indexes.Count - 1)
                r_index = 0;
            rightHandWeapon = rh_indexes[r_index];

            if (l_index > lh_indexes.Count - 1)
                l_index = 0;
            leftHandWeapon = lh_indexes[l_index];

            EquipWeapon(rightHandWeapon, false);
            EquipWeapon(leftHandWeapon, true);

            if (c_index > consum_indexes.Count - 1)
                c_index = 0;

            EquipConsumable(consum_indexes[c_index]);


            if (r_spells.Count > 0)
            {
                if (s_index > r_spells.Count - 1)
                    s_index = 0;

                EquipSpell(r_spells[s_index]);
            }
            else
            {
                uiSlot.ClearSlot(UI.QSlotType.spell);
            }
        }
        public RuntimeSpellItems SpellToRuntimeSpell(Spell s, bool isLeft = false)
        {
            if (s == null)
            {
                Debug.LogWarning("InventoryManager: Spell asset is null, skip adding runtime spell.");
                return null;
            }

            GameObject go = new GameObject();
            go.transform.parent = referenceParent.transform;
            RuntimeSpellItems inst = go.AddComponent<RuntimeSpellItems>();
            inst.instance = new Spell();
            StaticFunctions.DeepCopySpell(s, inst.instance);
            go.name = s.Item_id;

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
                Debug.LogWarning($"CreateSpellParticle: particlePrefab is null for spell '{inst.instance.Item_id}'");
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
            if (w == null)
            {
                Debug.LogError("InventoryManager: WeaponToRuntimeWeapon — weapon data is null.");
                return null;
            }

            GameObject go = new GameObject();
            go.transform.parent = referenceParent.transform;
            RuntimeWeapon inst = go.AddComponent<RuntimeWeapon>();
            go.name = w.Item_id;

            inst.instance = new Weapon();
            StaticFunctions.DeepCopyWeapon(w, inst.instance);

            inst.weaponStats = new WeaponStats();
            WeaponStats w_stats = ResourcesManager.singleton.GetWeaponStats(w.Item_id);
            StaticFunctions.DeepCopyWeaponStats(w_stats, inst.weaponStats);

            inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.weaponModel.transform.parent = p;


            if (!isLeft)
            {
                inst.weaponModel.transform.localPosition = Vector3.zero;
                inst.weaponModel.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                inst.weaponModel.transform.localPosition = leftHandPos;
                inst.weaponModel.transform.localEulerAngles = leftHandEuler;
            }

            inst.weaponModel.transform.localScale = Vector3.one;


            inst.w_Hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
            inst.w_Hook.InitDamageCollider(states);

            inst.weaponModel.SetActive(false);
            return inst;

        }
        public RuntimeConsumable ConsumableToRuntime(Consumable c)
        {
            GameObject go = new GameObject();
            go.transform.parent = referenceParent.transform;
            RuntimeConsumable inst = go.AddComponent<RuntimeConsumable>();
            go.name = c.Item_id;

            inst.instance = new Consumable();
            StaticFunctions.DeepCopyConsumable(c, inst.instance);


            if (inst.instance.itemPrefab != null)
            {
                GameObject model = Instantiate(inst.instance.itemPrefab) as GameObject;
                Transform p = states.anim.GetBoneTransform(HumanBodyBones.RightHand);
                model.transform.parent = p;
                model.transform.localPosition = Vector3.zero;
                model.transform.localEulerAngles = Vector3.zero;
                model.transform.localScale = Vector3.one;

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
                m_lh_weapons = leftHandWeapon.instance.Item_id;
            }
            else
            {
                if (rightHandWeapon != null)
                {
                    rightHandWeapon.weaponModel.SetActive(false);
                }
                rightHandWeapon = w;
                m_rh_weapons = rightHandWeapon.instance.Item_id;
            }
            String targetIdle = w.instance.oh_idle;
            targetIdle += isLeft ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            if (states.isLocal)
            {
                Item item = ResourcesManager.singleton.GetItem(w.instance.Item_id, ItemType.weapon);
                uiSlot.UpdateSlot((isLeft) ? UI.QSlotType.lh : UI.QSlotType.rh, item.GetIconId());
            }

            if (w.weaponModel)
                w.weaponModel.SetActive(true);
        }
        public void EquipSpell(RuntimeSpellItems spell)
        {
            currentSpell = spell;

            Item item = ResourcesManager.singleton.GetItem(spell.instance.Item_id, ItemType.spell);
            uiSlot.UpdateSlot(UI.QSlotType.spell, item.GetIconId());
        }
        public void EquipConsumable(RuntimeConsumable consum)
        {
            currentConsumable = consum;

            Item item = ResourcesManager.singleton.GetItem(consum.instance.Item_id, ItemType.consumable);
            uiSlot.UpdateSlot(UI.QSlotType.item, item.GetIconId());
        }
        public Weapon GetCurrentWeapon(bool isLeft)
        {
            if (isLeft)
                return leftHandWeapon.instance;
            else
                return rightHandWeapon.instance;
        }
        public RuntimeWeapon GetRuntimeWeapon(bool isLeft)
        {
            if (isLeft)
                return leftHandWeapon;
            else
                return rightHandWeapon;
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
            if (parryCollider)
                parryCollider.SetActive(false);
        }
        public void OpenParryCollider()
        {
            if (parryCollider)
                parryCollider.SetActive(true);
        }
        public void ChangeToNextWeapon(bool isLeft)
        {
            states.sendWeapons = true;
            states.isTwoHanded = false;
            states.HandleTwoHanded();

            if (isLeft)
            {
                if (lh_indexes.Count == 0)
                    return;

                if (l_index < lh_indexes.Count - 1)
                    l_index++;
                else
                    l_index = 0;

                EquipWeapon(lh_indexes[l_index], true);
            }
            else
            {
                if (rh_indexes.Count == 0)
                    return;

                if (r_index < rh_indexes.Count - 1)
                    r_index++;
                else
                    r_index = 0;

                EquipWeapon(rh_indexes[r_index]);
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
            if (c_index < consum_indexes.Count - 1)
                c_index++;
            else
                c_index = 0;

            EquipConsumable(consum_indexes[c_index]);
        }
        void MakeIndexesList()
        {
            consum_indexes.Clear();

            for (int i = 0; i < r_consum.Count; i++)
            {
                if (r_consum[i] == null || r_consum[i].isEmpty)
                    continue;

                consum_indexes.Add(r_consum[i]);
            }

            if (consum_indexes.Count < 2)
            {
                consum_indexes.Add(emptyItem);
            }

            rh_indexes.Clear();
            for (int i = 0; i < r_r_weapons.Count; i++)
            {
                if (r_r_weapons[i].isUnarmed)
                    continue;

                rh_indexes.Add(r_r_weapons[i]);
            }

            if (rh_indexes.Count < 2)
            {
                rh_indexes.Add(unarmedRuntime);
            }

            lh_indexes.Clear();
            for (int i = 0; i < r_l_weapons.Count; i++)
            {
                if (r_l_weapons[i].isUnarmed)
                    continue;

                lh_indexes.Add(r_l_weapons[i]);
            }

            if (lh_indexes.Count < 2)
            {
                lh_indexes.Add(unarmedRuntime);
            }
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
        public string Item_id;
        public string name_item;
        public string itemDescription;
        [Tooltip("Registry key. Leave empty to use normalized Item_id.")]
        public string skillDescription;
        public string iconId;
        public Sprite icon;

        public IconId GetIconId()
        {
            if (!string.IsNullOrWhiteSpace(iconId))
                return new IconId(IconId.Normalize(iconId));

            return IconId.FromItemName(Item_id);
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public string Item_id;
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handedActions;

        public float parryMultiplier;
        public float backstabMultiplier;
        public bool LeftHandMirror;

        public GameObject modelPrefab;

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
    public class Spell
    {
        public string Item_id;
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
    public class Consumable
    {
        public string Item_id;
        public string consumableEffect;
        public string targetAnim;
        public GameObject itemPrefab;
    }
    public class ArmorSnapshot
    {
        public string m_chestId;
        public string m_legsId;
        public string m_headId;
        public string m_handsId;
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

