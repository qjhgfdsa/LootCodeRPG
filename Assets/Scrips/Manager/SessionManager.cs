using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class SessionManager : MonoBehaviour
    {
        [Header("Equiped Items")]
        public List<string> rh_Equiped = new List<string>();
        public List<string> lh_Equiped = new List<string>();
        public List<string> con_Equiped = new List<string>();
        public List<string> spell_Equiped = new List<string>();
      
        [Header("In Inventory")]
        public List<string> weapon_items = new List<string>();
        public List<string> spell_items = new List<string>();
        public List<string> consumable_items = new List<string>();

        public List<Item> GetItemAsList(ItemType t)
        {
            switch (t)
            {
                case ItemType.weapon:
                    return ResourcesManager.singleton.GetAllItemsFromList(weapon_items, t);
                case ItemType.spell:
                    return ResourcesManager.singleton.GetAllItemsFromList(spell_items, t);
                case ItemType.consumable:
                    return ResourcesManager.singleton.GetAllItemsFromList(consumable_items, t);
                case ItemType.equipment:
                default:
                    return null;
            }
        }

        public static SessionManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
