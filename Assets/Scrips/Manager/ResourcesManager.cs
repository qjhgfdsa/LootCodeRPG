using System.Collections.Generic;
using SA;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
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
            if (spell_ids.ContainsKey(obj.spell_items[i].itemName))
            {
                Debug.Log(obj.spell_items[i].itemName + "Item is a duplicate");
            }
            else
            {
                spell_ids.Add(obj.spell_items[i].itemName, i);
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
            if (weapon_ids.ContainsKey(obj.weapons_all[i].itemName))
            {
                Debug.Log(obj.weapons_all[i].itemName + "item is duplicate");
            }
            else
            {
                weapon_ids.Add(obj.weapons_all[i].itemName, i);
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
            if (consumable_ids.ContainsKey(obj.consumables[i].itemName))
            {
                Debug.Log(obj.consumables[i].itemName + "Item is a duplicate");
            }
            else
            {
                consumable_ids.Add(obj.consumables[i].itemName, i);
            }
        }
    }
    int GetWeaponIdFromString(string id)
    {
        int index = -1;
        if (weapon_ids.TryGetValue(id, out index))
        {
            return index;
        }
        return -1;
    }

    int GetWeaponStatsIdFromString(string id)
    {
        int index = -1;
        if (weaponStats_ids.TryGetValue(id, out index))
        {
            return index;
        }
        return -1;
    }

    public Weapon GetWeapon(string id)
    {
        WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;
        int index = GetWeaponIdFromString(id);
        if (obj == null)
        {
            Debug.Log("SA.WeaponScriptableObject cant be loaded!");
            return null;
        }

        if (index == -1)
            return null;

        return obj.weapons_all[index];
    }

    public WeaponStats GetWeaponStats(string id)
    {
        WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;
        int index = GetWeaponStatsIdFromString(id);
        if (obj == null)
        {
            Debug.Log("SA.WeaponStatsScriptableObject cant be loaded!");
            return null;
        }

        if (index == -1)
            return null;

        return obj.weaponStats_all[index];
    }

    int GetSpellIdFromString(string id)
    {
        int index = -1;
        if (spell_ids.TryGetValue(id, out index))
        {
            return index;
        }
        return index;
    }

    public Spell GetSpell(string id)
    {
        SpellItemScriptableObject obj = Resources.Load("SA.SpellItemScriptableObject") as SpellItemScriptableObject;
        if (obj == null)
        {
            Debug.Log("SA.SpellItemScriptableObject cant be loaded!");
            return null;
        }
        int index = GetSpellIdFromString(id);
        if (index == -1)
            return null;

        return obj.spell_items[index];
    }
    int GetConsumableIdFromString(string id)
    {
        int index = -1;
        if (consumable_ids.TryGetValue(id, out index))
        {
            return index;
        }
        return index;
    }
    public Consumable GetConsumable(string id)
    {
        ConsumablesScriptableObject obj = Resources.Load("SA.ConsumablesScriptableObject") as ConsumablesScriptableObject;
        if (obj == null)
        {
            Debug.Log("SA.ConsumablesScriptableObject cant be loaded!");
            return null;
        }
        int index = GetConsumableIdFromString(id);
        if (index == -1)
            return null;
        return obj.consumables[index];
    }
}
