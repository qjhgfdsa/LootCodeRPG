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

        public string a_chestPiece;
        public string a_legsPiece;
        public string a_handsPiece;
        public string a_headPiece;

        [HideInInspector]
        public int _a_chest;
        [HideInInspector]
        public int _a_legs;
        [HideInInspector]
        public int _a_hands;
        [HideInInspector]
        public int _a_head;


        [HideInInspector]
        public List<int> _eq_rh = new List<int>();
        [HideInInspector]
        public List<int> _eq_lh = new List<int>();
        [HideInInspector]
        public List<int> _eq_con = new List<int>();

        [Header("In Inventory")]
        public List<string> weapon_items = new List<string>();
        public List<string> spell_items = new List<string>();
        public List<string> consumable_items = new List<string>();
        public List<string> armor_items = new List<string>();

        int m_w_item_index;
        int m_c_item_index;
        int m_a_item_index;
        List<ItemInventoryInstance> _w_items = new List<ItemInventoryInstance>();
        List<ItemInventoryInstance> _c_items = new List<ItemInventoryInstance>();
        List<ItemInventoryInstance> _a_items = new List<ItemInventoryInstance>();
        ItemInventoryInstance unarmedItem = new ItemInventoryInstance();
        ItemInventoryInstance emptyItem = new ItemInventoryInstance();



        public List<ItemInventoryInstance> GetItemsIntanceList(ItemType t)
        {
            switch (t)
            {
                case ItemType.weapon:
                    return _w_items;

                case ItemType.consumable:
                    return _c_items;

                case ItemType.spell:
                    return null;
                case ItemType.equipment:
                    return _a_items;
                default:
                    return null;
            }
        }
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
        public ItemInventoryInstance GetArmorItem(int uniqueId)
        {
            if (uniqueId == -1)

            {
                return emptyItem;
            }

            return GetItem(_a_items, uniqueId);
        }
        public ItemInventoryInstance GetWeaponItem(int uniqueId)
        {
            if (uniqueId == -1)
                return unarmedItem;

            return GetItem(_w_items, uniqueId);
        }
        public ItemInventoryInstance GetConItem(int uniqueId)
        {
            if (uniqueId == -1)
                return emptyItem;

            return GetItem(_c_items, uniqueId);
        }
        public ItemInventoryInstance GetItem(List<ItemInventoryInstance> l, int uniqueId)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].uniqueId == uniqueId)
                    return l[i];
            }
            return null;
        }
        public ItemInventoryInstance StringToItemInst(List<ItemInventoryInstance> l, string s)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (string.Equals(l[i].itemId, s))
                    return l[i];
            }
            return null;
        }
        public UI.InventoryUI inventoryUI;
        public ResourcesManager rm;
        public static SessionManager singleton;

        void InitEmpties()
        {
            unarmedItem = new ItemInventoryInstance();
            unarmedItem.itemId = "มือเปล่า";
            unarmedItem.uniqueId = -1;

            emptyItem = new ItemInventoryInstance();
            emptyItem.itemId = "empty";
            emptyItem.uniqueId = -1;

        }
        void Awake()
        {
            singleton = this;

            InitEmpties();

            rm.Pre_Init();
            inventoryUI.Pre_Init();
            #region Equipments
            for (int i = 0; i < rh_Equiped.Count; i++)
            {
                weapon_items.Add(rh_Equiped[i]);
            }
            for (int i = 0; i < lh_Equiped.Count; i++)
            {
                weapon_items.Add(lh_Equiped[i]);
            }
            for (int i = 0; i < con_Equiped.Count; i++)
            {
                consumable_items.Add(con_Equiped[i]);
            }

            for (int i = 0; i < weapon_items.Count; i++)
            {
                ItemInventoryInstance it = new ItemInventoryInstance();
                it.itemId = weapon_items[i];
                it.uniqueId = m_w_item_index;
                m_w_item_index++;
                _w_items.Add(it);
            }


            for (int i = 0; i < rh_Equiped.Count; i++)
            {
                ItemInventoryInstance it = StringToItemInst(_w_items, rh_Equiped[i]);
                _eq_rh.Add(it.uniqueId);
                it.slot = inventoryUI.equipmentSlotUI.GetWeaponSlot(i);
                it.eq_index = i;
            }
            for (int i = 0; i < lh_Equiped.Count; i++)
            {
                ItemInventoryInstance it = StringToItemInst(_w_items, lh_Equiped[i]);
                _eq_lh.Add(it.uniqueId);
                int targetIndex = i + 3;
                it.slot = inventoryUI.equipmentSlotUI.GetWeaponSlot(targetIndex);
                it.eq_index = targetIndex;
            }

            for (int i = 0; i < consumable_items.Count; i++)
            {
                ItemInventoryInstance it = new ItemInventoryInstance();
                it.itemId = consumable_items[i];
                it.uniqueId = m_c_item_index;
                m_c_item_index++;
                _c_items.Add(it);
            }

            for (int i = 0; i < con_Equiped.Count; i++)
            {
                ItemInventoryInstance it = StringToItemInst(_c_items, con_Equiped[i]);
                _eq_con.Add(it.uniqueId);
                it.slot = inventoryUI.equipmentSlotUI.GetConSlot(i);
                it.eq_index = i;
            }
            #endregion

            AddArmorItem(a_chestPiece, ArmorType.chest, true);
            AddArmorItem(a_legsPiece, ArmorType.legs, true);
            AddArmorItem(a_handsPiece, ArmorType.hands, true);
            AddArmorItem(a_headPiece, ArmorType.head, true);

            for (int i = 0; i < armor_items.Count; i++)
            {
                AddArmorItem(armor_items[i]);
            }
        }
        void AddArmorItem(string id, ArmorType t = ArmorType.chest, bool isEquipped = false)
        {
            if (string.IsNullOrEmpty(id) || string.Equals(id, "empty"))
            {
                if (isEquipped)
                {
                    switch (t)
                    {
                        case ArmorType.chest:
                            _a_chest = -1;
                            break;
                        case ArmorType.legs:
                            _a_legs = -1;
                            break;
                        case ArmorType.hands:
                            _a_hands = -1;
                            break;
                        case ArmorType.head:
                            _a_head = -1;
                            break;
                        default:
                            break;
                    }
                }
                return;
            }
            ItemInventoryInstance item = new ItemInventoryInstance();
            item.itemId = id;
            m_a_item_index++;
            item.uniqueId = m_a_item_index;
            _a_items.Add(item);

            if (isEquipped)
            {
                switch (t)
                {
                    case ArmorType.chest:
                        _a_chest = item.uniqueId;
                        break;
                    case ArmorType.legs:
                        _a_legs = item.uniqueId;
                        break;
                    case ArmorType.hands:
                        _a_hands = item.uniqueId;
                        break;
                    case ArmorType.head:
                        _a_head = item.uniqueId;
                        break;
                    default:
                        break;
                }
                //  item.armorType = t;
            }
        }
       public void AddItem(string id, ItemType t)
        {

            switch (t)
            {
                case ItemType.weapon:
                    weapon_items.Add(id);
                    ItemInventoryInstance it = new ItemInventoryInstance();
                    it.itemId = id;
                    it.uniqueId = m_w_item_index;
                    m_w_item_index++;
                    _w_items.Add(it);
                    break;
                case ItemType.consumable:
                    consumable_items.Add(id);
                    ItemInventoryInstance c_it = new ItemInventoryInstance();
                    c_it.itemId = id;
                    c_it.uniqueId = m_c_item_index;
                    m_c_item_index++;
                    _c_items.Add(c_it);
                    break;
                case ItemType.equipment:
                    armor_items.Add(id);
                    ItemInventoryInstance a_it = new ItemInventoryInstance();
                    a_it.itemId = id;
                    a_it.uniqueId = m_a_item_index;
                    m_a_item_index++;
                    _a_items.Add(a_it);
                    break;
            }
            Item i = rm.GetItem(id, t);
            UIManager.singleton.AddAnnounceCard(i);
        }
    }

    [System.Serializable]
    public class ItemInventoryInstance
    {
        public int uniqueId;
        public int eq_index;
        public string itemId;
        // public ArmorType armorType;
        public UI.EquipmentSlot slot;
    }
}
