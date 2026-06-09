using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class SessionManager : MonoBehaviour
    {
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
