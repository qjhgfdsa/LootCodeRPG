using System.Collections.Generic;
using SA;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    Dictionary<string, int> i_spells = new Dictionary<string, int>();
    Dictionary<string, int> i_weapons = new Dictionary<string, int>();
    Dictionary<string, int> i_consumables = new Dictionary<string, int>();



    Dictionary<string, int> spell_ids = new Dictionary<string, int>();
    Dictionary<string, int> weapon_ids = new Dictionary<string, int>();
    Dictionary<string, int> weaponStats_ids = new Dictionary<string, int>();
    Dictionary<string, int> consumable_ids = new Dictionary<string, int>();
    public static ResourcesManager singleton;
    void Awake()
    {
        singleton = this;
        LoadWeaponId();
        LoadSpellIds();
        LoadConsumableId();
        LoadItems();
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
    int GetIndexFromString(Dictionary<string, int> d, string id)
    {
        int index = -1;
        d.TryGetValue(id, out index);
        return index;
    }
    public enum ItemType
    {
        weapon, spell, consumable, equipment
    }
    public Item GetItem(string id, ItemType type)
    {
        ItemScriptableObjectScript obj = Resources.Load("SA.ItemScriptableObjectScript") as ItemScriptableObjectScript;

        int index = GetIndexFromString(i_weapons, id);
        if (index == -1)
            return null;
    
        switch (type)
        {
            case ItemType.weapon:
                return obj.weapon_items[index];
            case ItemType.spell:
                return obj.spell_items[index];
            case ItemType.consumable:
                return obj.cons_items[index];
            case ItemType.equipment:
            default:
                return null;
        }

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
}
