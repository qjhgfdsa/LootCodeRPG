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
        public PlayerStatus playerStatus;

        public GameObject gameMenu, inventoryMenu, centerMain, centerRight, centerOverlay;

        void Start()
        {
            CreateUIElements();
        }
        void CreateUIElements()
        {
            weaponInfoInit();
            PlayerStatusInit();
        }
        void weaponInfoInit()
        {
            for (int i = 0; i < 6; i++)
            {
                CreateAttDefUIElement(weaponInfo.ap_slots, weaponInfo.ap_Grid, (AttackDefenseType)i);
            }

            for (int i = 0; i < 5; i++)
            {
                CreateAttDefUIElement(weaponInfo.g_absorb, weaponInfo.g_absorb_Grid, (AttackDefenseType)i);
            }

            CreateAttDefUIElement(weaponInfo.g_absorb, weaponInfo.g_absorb_Grid, AttackDefenseType.stability);

            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_Grid, AttackDefenseType.bleed);
            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_Grid, AttackDefenseType.curse);
            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_Grid, AttackDefenseType.frost);
            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_Grid, AttackDefenseType.magicBuff);


            CreateAttributeUIElement_Mini(weaponInfo.att_bonus, weaponInfo.att_bonus_Grid, AttributeType.strength);
            CreateAttributeUIElement_Mini(weaponInfo.att_bonus, weaponInfo.att_bonus_Grid, AttributeType.dexterity);
            CreateAttributeUIElement_Mini(weaponInfo.att_bonus, weaponInfo.att_bonus_Grid, AttributeType.intelligence);
            CreateAttributeUIElement_Mini(weaponInfo.att_bonus, weaponInfo.att_bonus_Grid, AttributeType.faith);


            CreateAttributeUIElement_Mini(weaponInfo.att_req, weaponInfo.att_req_Grid, AttributeType.vigor);
            CreateAttributeUIElement_Mini(weaponInfo.att_req, weaponInfo.att_req_Grid, AttributeType.attunement);
            CreateAttributeUIElement_Mini(weaponInfo.att_req, weaponInfo.att_req_Grid, AttributeType.endurance);
            CreateAttributeUIElement_Mini(weaponInfo.att_req, weaponInfo.att_req_Grid, AttributeType.vitality);

        }
        void PlayerStatusInit()
        {
            CreateAttributeUIElement(playerStatus.attSlot, playerStatus.Grid,AttributeType.level);
            CreateEmptySlot(playerStatus.Grid);

            for (int i = 0; i < 8; i++)
            {
                CreateAttributeUIElement(playerStatus.attSlot, playerStatus.Grid, (AttributeType)i);
            }

            CreateEmptySlot(playerStatus.Grid);
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                index += 8;
                CreateAttributeUIElement(playerStatus.attSlot, playerStatus.Grid, (AttributeType)index);
            }

            
            CreateEmptySlot(playerStatus.Grid);
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                index += 11;
                CreateAttributeUIElement(playerStatus.attSlot, playerStatus.Grid, (AttributeType)index);
            }




        }
        void CreateAttDefUIElement(List<AttDefType> l, Transform p, AttackDefenseType t)
        {
            AttDefType a = new AttDefType();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slots_template) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.txt1.text = a.type.ToString();
            a.slot.txt2.text = "99";
            g.SetActive(true);
        }
        void CreateAttDefUIElement_Mini(List<AttDefType> l, Transform p, AttackDefenseType t)
        {
            AttDefType a = new AttDefType();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slot_mini) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.txt1.text = "0";
            g.SetActive(true);
        }
        void CreateAttributeUIElement_Mini(List<AttributeSlot> l, Transform p, AttributeType t)
        {
            AttributeSlot a = new AttributeSlot();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slot_mini) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.txt1.text = "-";
            g.SetActive(true);

        }
        void CreateAttributeUIElement(List<AttributeSlot> l, Transform p, AttributeType t)
        {
            AttributeSlot a = new AttributeSlot();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(playerStatus.slot_template) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.txt1.text = t.ToString();
            a.slot.txt2.text = "99";
            g.SetActive(true);
        }
        void CreateEmptySlot(Transform p)
        {
            GameObject g = Instantiate(playerStatus.emptySlot) as GameObject;
            g.transform.SetParent(p);
            g.SetActive(true);
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
        public class PlayerStatus
        {
            public GameObject slot_template;
            public GameObject emptySlot;
            public Transform Grid;
            public List<AttributeSlot> attSlot = new List<AttributeSlot>();
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
            public GameObject slot_mini;
            public GameObject breakSlot;
            public TMP_Text itemName;
            public TMP_Text weaponType;
            public TMP_Text damageType;
            public TMP_Text skillName;
            public TMP_Text fpCost;
            public TMP_Text weightCost;
            public TMP_Text durability_min;
            public TMP_Text durability_max;

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
            public InventoryUISlot slot;


        }



    }
}
