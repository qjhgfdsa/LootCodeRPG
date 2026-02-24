using System.Collections.Generic;
using SA;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public List<Weapon> weaponList = new List<Weapon>();
    Dictionary<string, int> weapon_dict = new Dictionary<string, int>();



    public static ResourcesManager singleton;
    void Awake()
    {
        singleton = this;

        for (int i = 0; i < weaponList.Count; i++)
        {
            if(string.IsNullOrEmpty(weaponList[i].weaponId))
            {
                continue;
            }

            if (!weapon_dict.ContainsKey(weaponList[i].weaponId))
            {
                weapon_dict.Add(weaponList[i].weaponId, i);
            }
            else
            {
                Debug.Log(weaponList[i].weaponId + " is a duplicate id");
            }
        }
    }

    public Weapon GetWeapon(string id)
    {
        int index = -1;
        if (weapon_dict.TryGetValue(id,out index))
        {
            return weaponList[index];
        }

        return null;
    }
   
    
}
