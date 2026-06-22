using System.Collections.Generic;
using SA;
using UnityEngine;

namespace SA
{
    public class ResourcesManager : MonoBehaviour
    {
        Dictionary<string, int> i_spells = new Dictionary<string, int>();
        Dictionary<string, int> i_weapons = new Dictionary<string, int>();
        Dictionary<string, int> i_consumables = new Dictionary<string, int>();
        Dictionary<string, int> i_armors = new Dictionary<string, int>();

        Dictionary<string, int> spell_ids = new Dictionary<string, int>();
        Dictionary<string, int> weapon_ids = new Dictionary<string, int>();
        Dictionary<string, int> weaponStats_ids = new Dictionary<string, int>();
        Dictionary<string, int> consumable_ids = new Dictionary<string, int>();
        Dictionary<string, int> armor_ids = new Dictionary<string, int>();
        public static ResourcesManager singleton;

        #region Load Items
        public void Pre_Init()
        {
            singleton = this;
            LoadItems();
            LoadWeaponId();
            LoadSpellIds();
            LoadConsumableId();
            LoadArmor();
        }
        void LoadArmor()
        {
            ArmorScriptableObjectScript obj = Resources.Load("SA.ArmorScriptableObjectScript") as ArmorScriptableObjectScript;
            if (obj == null)
            {
                Debug.Log("SA.ArmorScriptableObjectScript หาไม่เจอ");
                return;
            }
            for (int i = 0; i < obj.armor_containers.Length; i++)
            {
                if (armor_ids.ContainsKey(obj.armor_containers[i].itemId))
                {
                    Debug.Log(obj.armor_containers[i].itemId + "Item is a duplicate");
                }
                else
                {
                    armor_ids.Add(obj.armor_containers[i].itemId, i);
                }
            }
        }
        void LoadItems()
        {
            ItemScriptableObjectScript obj = Resources.Load("SA.ItemScriptableObjectScript") as ItemScriptableObjectScript;
            if (obj == null)
            {
                Debug.Log("SA.ItemScriptableObjectScript หาไม่เจอ");
                return;
            }

            for (int i = 0; i < obj.spell_items.Count; i++)
            {
                if (i_spells.ContainsKey(obj.spell_items[i].Item_id))
                {
                    Debug.Log(obj.spell_items[i].Item_id + "Item is a duplicate");
                }
                else
                {
                    i_spells.Add(obj.spell_items[i].Item_id, i);
                }
            }
            for (int i = 0; i < obj.weapon_items.Count; i++)
            {
                if (i_weapons.ContainsKey(obj.weapon_items[i].Item_id))
                {
                    Debug.Log(obj.weapon_items[i].Item_id + "Item is a duplicate");
                }
                else
                {
                    i_weapons.Add(obj.weapon_items[i].Item_id, i);
                }
            }
            for (int i = 0; i < obj.cons_items.Count; i++)
            {
                if (i_consumables.ContainsKey(obj.cons_items[i].Item_id))
                {
                    Debug.Log(obj.cons_items[i].Item_id + "Item is a duplicate");
                }
                else
                {
                    i_consumables.Add(obj.cons_items[i].Item_id, i);

                }
            }
            for (int i = 0; i < obj.armor_items.Count; i++)
            {
                if (i_armors.ContainsKey(obj.armor_items[i].Item_id))
                {
                    Debug.Log(obj.armor_items[i].Item_id + "Item is a duplicate");
                }
                else
                {
                    i_armors.Add(obj.armor_items[i].Item_id, i);
                }
            }
        }
        void LoadSpellIds()
        {
            SpellItemScriptableObject obj = Resources.Load("SA.SpellItemScriptableObject") as SpellItemScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.SpellItemScriptableObject หาไม่เจอ");
                return;
            }
            for (int i = 0; i < obj.spell_items.Count; i++)
            {
                if (spell_ids.ContainsKey(obj.spell_items[i].Item_id))
                {
                    Debug.Log(obj.spell_items[i].Item_id + "Item is a duplicate");
                }
                else
                {
                    spell_ids.Add(obj.spell_items[i].Item_id, i);
                }
            }
        }
        void LoadWeaponId()
        {
            WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.WeaponScriptableObject หาไม่เจอ");
                return;
            }

            for (int i = 0; i < obj.weapons_all.Count; i++)
            {
                if (weapon_ids.ContainsKey(obj.weapons_all[i].Item_id))
                {
                    Debug.Log(obj.weapons_all[i].Item_id + "item is duplicate");
                }
                else
                {
                    weapon_ids.Add(obj.weapons_all[i].Item_id, i);
                }
            }
            for (int i = 0; i < obj.weaponStats_all.Count; i++)
            {
                if (weaponStats_ids.ContainsKey(obj.weaponStats_all[i].weaponId))
                {
                    Debug.Log(obj.weaponStats_all[i].weaponId + "weaponStats is duplicate");
                }
                else
                {
                    weaponStats_ids.Add(obj.weaponStats_all[i].weaponId, i);
                }
            }
        }
        void LoadConsumableId()
        {
            ConsumablesScriptableObject obj = Resources.Load("SA.ConsumablesScriptableObject") as ConsumablesScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.ConsumablesScriptableObject หาไม่เจอ");
                return;
            }
            for (int i = 0; i < obj.consumables.Count; i++)
            {
                if (consumable_ids.ContainsKey(obj.consumables[i].Item_id))
                {
                    Debug.Log(obj.consumables[i].Item_id + "Item is a duplicate");
                }
                else
                {
                    consumable_ids.Add(obj.consumables[i].Item_id, i);
                }
            }
        }
        #endregion

        public List<Item> GetAllItemsFromList(List<string> l, ItemType t)
        {
            List<Item> r = new List<Item>();
            for (int i = 0; i < l.Count; i++)
            {
                Item it = GetItem(l[i], t);
                r.Add(it);
            }
            return r;
        }
        int GetIndexFromString(Dictionary<string, int> d, string id)
        {
            int index = -1;
            d.TryGetValue(id, out index);
            return index;
        }
        public Item GetItem(string id, ItemType type)
        {
            ItemScriptableObjectScript obj = Resources.Load("SA.ItemScriptableObjectScript") as ItemScriptableObjectScript;

            if (obj == null)
            {
                Debug.Log("SA.ItemScriptableObjectScript cant be loaded!");
            }

            Dictionary<string, int> d = null;
            List<Item> l = null;

            switch (type)
            {
                case ItemType.weapon:
                    d = i_weapons;
                    l = obj.weapon_items;
                    break;
                case ItemType.spell:
                    d = i_spells;
                    l = obj.spell_items;
                    break;
                case ItemType.consumable:
                    d = i_consumables;
                    l = obj.cons_items;
                    break;
                case ItemType.equipment:
                    d = i_armors;
                    l = obj.armor_items;
                    break;
                default:
                    break;
            }

            if (d == null)
                return null;
            if (l == null)
                return null;

            int index = GetIndexFromString(d, id);
            if (index == -1)
                return null;

            return l[index];

        }
        public Weapon GetWeapon(string id)
        {
            WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;
            if (obj == null)
            {
                Debug.Log("SA.WeaponScriptableObject cant be loaded!");
                return null;
            }
            // int index = GetWeaponIdFromString(id);
            int index = GetIndexFromString(weapon_ids, id);
            if (index == -1)
                return null;

            return obj.weapons_all[index];
        }
        public WeaponStats GetWeaponStats(string id)
        {
            WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.WeaponStatsScriptableObject cant be loaded!");
                return null;
            }

            int index = GetIndexFromString(weaponStats_ids, id);

            if (index == -1)
                return null;

            return obj.weaponStats_all[index];
        }
        public Spell GetSpell(string id)
        {
            SpellItemScriptableObject obj = Resources.Load("SA.SpellItemScriptableObject") as SpellItemScriptableObject;
            if (obj == null)
            {
                Debug.Log("SA.SpellItemScriptableObject cant be loaded!");
                return null;
            }

            int index = GetIndexFromString(spell_ids, id);
            if (index == -1)
                return null;

            return obj.spell_items[index];
        }
        public Consumable GetConsumable(string id)
        {
            ConsumablesScriptableObject obj = Resources.Load("SA.ConsumablesScriptableObject") as ConsumablesScriptableObject;
            if (obj == null)
            {
                Debug.Log("SA.ConsumablesScriptableObject cant be loaded!");
                return null;
            }
            int index = GetIndexFromString(consumable_ids, id);
            if (index == -1)
                return null;
            return obj.consumables[index];
        }
        public ArmorContainer GetArmor(string id)
        {
            ArmorScriptableObjectScript obj = Resources.Load("SA.ArmorScriptableObjectScript") as ArmorScriptableObjectScript;
            if (obj == null)
            {
                Debug.Log("SA.ArmorScriptableObjectScript cant be loaded!");
                return null;
            }
            int index = GetIndexFromString(armor_ids, id);
            if (index == -1)
                return null;
            return obj.armor_containers[index];
        }
    }
    public enum ItemType
    {
        weapon, spell, consumable, equipment
    }

}

