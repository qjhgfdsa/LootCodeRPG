using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SA;
using System.Collections.Generic;
using System;

namespace SA.UI
{
    public class InventoryUI : MonoBehaviour
    {
        public EquipmentLeft eq_left;
        public CenterOverlay c_overlay;
        public WeaponInfo weaponInfo;

        public GameObject gameMenu, inventoryMenu, centerMain, centerRight, centerOverlay;

        void Start()
        {
            CreateUIElements();


        }
        void CreateUIElements()
        {
            for (int i = 0; i < weaponInfo.itemDetails.Count; i++)
            {
                AttributeSlot a = weaponInfo.itemDetails[i];
                if (a.isBreak)
                {
                    GameObject b = Instantiate(weaponInfo.breakSlot) as GameObject;
                    b.transform.SetParent(weaponInfo.id_Grid);
                    continue;
                }
                GameObject g = Instantiate(weaponInfo.slots_template) as GameObject;
                g.transform.SetParent(weaponInfo.id_Grid);
                a.slot = g.GetComponent<InventoryUISlot>();
                a.slot.txt1.text = a.type.ToString();
                a.slot.txt2.text = "99";



            }

        }

        public UIState curUIState;
        public void Tick()
        {

        }
        public static InventoryUI singleton;
        public void Awake()
        {
            singleton = this;
        }

        public enum UIState
        {
            eqipment, inventory, attributes, messages, options
        }
        [System.Serializable]
        public class EquipmentLeft
        {
            public TMP_Text slotName;
            public TMP_Text curItemName;
            public Left_Inventory inventory;
            public EquipmentSlots slots;
        }
        [System.Serializable]
        public class Left_Inventory
        {
            public Slider inventorySlider;
            public GameObject slotTemplate;
            public Transform slotGrid;
        }
        [System.Serializable]
        public class EquipmentSlots
        {

        }
        [System.Serializable]
        public class CenterOverlay
        {
            public Image bigIcon;
            public TMP_Text itemName;
            public TMP_Text itemDescription;
            public TMP_Text skillName;
            public TMP_Text skillDescription;
        }

        [System.Serializable]
        public class WeaponInfo
        {
            public Image smallIcon;

            public GameObject slots_template;
            public GameObject breakSlot;
            public Transform id_Grid;
            public List<AttributeSlot> itemDetails = new List<AttributeSlot>();
            public Transform ap_Grid;
            public List<AttDefType> ap_slots = new List<AttDefType>();
            public Transform g_absorb_Grid;
            public List<AttDefType> g_absorb = new List<AttDefType>();
            public Transform a_effects_Grid;
            public List<AttDefType> a_effects = new List<AttDefType>();
            public Transform att_bonus_Grid;
            public List<AttributeSlot> att_bonus = new List<AttributeSlot>();
            public Transform att_req_Grid;
            public List<AttributeSlot> att_req = new List<AttributeSlot>();

            public AttributeSlot GetAttslot(List<AttributeSlot> l, AttributeType type)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].type == type)
                    {
                        return l[i];
                    }
                }
                return null;
            }
            public AttDefType GetAttDefSlot(List<AttDefType> l, AttackDefenseType type)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].type == type)
                    {
                        return l[i];
                    }
                }
                return null;
            }

        }
        [System.Serializable]
        public class ItemDetails
        {

        }

        [System.Serializable]
        public class AttributeSlot
        {
            public bool isBreak;
            public AttributeType type;
            public InventoryUISlot slot;

        }
         [System.Serializable]
        public class AttDefType
        {
            public bool isBreak;
            public AttackDefenseType type;
            public Image icon;
            public TMP_Text attName0;
            public TMP_Text attNumber;

        }



    }
}
