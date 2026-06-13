using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;



namespace SA.UI
{
    public class InventoryUI : MonoBehaviour
    {
        public EquipmentLeft eq_left;
        public CenterOverlay c_overlay;
        public WeaponInfo weaponInfo;
        public PlayerStatus playerStatus;


        public GameObject gameMenu, inventoryMenu, centerMain, centerRight, centerOverlay;

        List<IconBase> iconSlotsCreated = new List<IconBase>();
        public EquipmentSlotUI equipmentSlotUI;

        public Transform equipmentParent;
        // List<EquipmentSlot> equipSlots = new List<EquipmentSlot>();
        EquipmentSlot[,] equipSlots;


        public Vector2 curSlotPos;
        float inputT;
        bool dontMove;

        public Color unselected;
        public Color selected;
        EquipmentSlot curEqSlot;
        EquipmentSlot prevEqSlot;
        float inpTimer;
        float moveTimer = 0.4f;
        void HandleSlotMovement()
        {
            int x = Mathf.RoundToInt(curSlotPos.x);
            int y = Mathf.RoundToInt(curSlotPos.y);

            bool up = (Input.GetAxis(StaticStrings.Vertical) > 0);
            bool down = (Input.GetAxis(StaticStrings.Vertical) < 0);
            bool left = (Input.GetAxis(StaticStrings.Horizontal) < 0);
            bool right = (Input.GetAxis(StaticStrings.Horizontal) > 0);

            if (!up && !down && !left && !right)
            {
                inpTimer = 0;

            }
            else
            {
                inpTimer -= Time.deltaTime;
            }

            if (inpTimer < 0)
                inpTimer = 0;

            if (inpTimer > 0)
                return;

            if (up)
            {
                y--;
                inpTimer = moveTimer;
            }

            if (down)
            {
                y++;
                inpTimer = moveTimer;
            }
            if (left)
            {
                x--;
                inpTimer = moveTimer;
            }
            if (right)
            {
                x++;
                inpTimer = moveTimer;
            }

            if (x > 4)
                x = 0;
            if (x < 0)
                x = 4;
            if (y > 5)
                y = 0;
            if (y < 0)
                y = 5;

            if (curEqSlot != null)
                curEqSlot.icon.background.color = unselected;

            if (x == 4 && y == 3)
            {
                x = 4;
                y = 2;
            }

            curEqSlot = equipSlots[x, y];
            curSlotPos.x = x;
            curSlotPos.y = y;
            if (curEqSlot != null)
                curEqSlot.icon.background.color = selected;
        }


        #region Create UI Elements
        void Start()
        {
            CreateUIElements();
            InitEqSlots();
        }
        void InitEqSlots()
        {
            EquipmentSlot[] eq = equipmentParent.GetComponentsInChildren<EquipmentSlot>();
            equipSlots = new EquipmentSlot[5, 6];

            for (int i = 0; i < eq.Length; i++)
            {
                eq[i].Init(this);
                int x = Mathf.RoundToInt(eq[i].slotPos.x);
                int y = Mathf.RoundToInt(eq[i].slotPos.y);
                equipSlots[x, y] = eq[i];
            }
        }
        void CreateUIElements()
        {
            weaponInfoInit();
            PlayerStatusInit();
            WeaponStatusInit();
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
            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_Grid, AttackDefenseType.poison);


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
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.level, "Level");
            CreateEmptySlot(playerStatus.attGrid);

            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.vigor, "Vigor");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.endurance, "Endurance");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.vitality, "Vitality");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.strength, "Strength");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.dexterity, "Dexterity");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.intelligence, "Intelligence");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.faith, "Faith");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.luck, "Luck");


            CreateEmptySlot(playerStatus.attGrid);

            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.hp, "HP");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.fp, "FP");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.stamina, "Stamina");

            CreateEmptySlot(playerStatus.attGrid);
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.equip_load, "Equip Load");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.poise, "Poise");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.item_discover, "Item Discover");
            CreateAttributeUIElement(playerStatus.attSlots, playerStatus.attGrid, AttributeType.attunement, "Attunement");

        }
        void WeaponStatusInit()
        {
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.physical, "Physical");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.strike, "VS Strike");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.slash, "VS Slash");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.thrust, "VS Thrust");

            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.magic, "Magic");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.fire, "Fire");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.lightning, "Lightning");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.dark, "Dark");

            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.bleed, "Bleed");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.curse, "Curse");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.frost, "Frost");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.poison, "Poison");

            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "R Weapon 1");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "R Weapon 2");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "R Weapon 3");

            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "L Weapon 1");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "L Weapon 2");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "L Weapon 3");


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
        void CreateAttributeUIElement(List<AttributeSlot> l, Transform p, AttributeType t, string txt1Text = null)
        {
            AttributeSlot a = new AttributeSlot();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(playerStatus.slot_template) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            if (string.IsNullOrEmpty(txt1Text))
                a.slot.txt1.text = t.ToString();
            else
                a.slot.txt1.text = txt1Text;
            a.slot.txt2.text = "99";
            g.SetActive(true);
        }
        void CreateEmptySlot(Transform p)
        {
            GameObject g = Instantiate(playerStatus.emptySlot) as GameObject;
            g.transform.SetParent(p);
            g.SetActive(true);
        }
        void CreateWeaponStatusSlot(List<PlayerStatusDef> l, Transform p, AttackDefenseType t, string txt1Text = null)
        {
            PlayerStatusDef w = new PlayerStatusDef();
            GameObject g = Instantiate(playerStatus.doubleSlot_template) as GameObject;
            g.SetActive(true);
            g.transform.SetParent(p);
            w.type = t;
            w.slot = g.GetComponent<InventoryUIDoubleSlot>();
            if (string.IsNullOrEmpty(txt1Text))
                w.slot.txt1.text = t.ToString();
            else
                w.slot.txt1.text = txt1Text;
            w.slot.txt2.text = "99";
            w.slot.txt3.text = "99";

        }
        void CreateAttackPowerSlot(List<AttackPowerSlot> l, Transform p, string id)
        {
            AttackPowerSlot a = new AttackPowerSlot();
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slots_template) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.txt1.text = id;
            a.slot.txt2.text = "99";
            g.SetActive(true);

        }

        #endregion
        public UIState curUIState;

        public void LoadCurrentItems(ItemType t)
        {
            List<Item> itemList = SessionManager.singleton.GetItemAsList(t);

            if (itemList == null)
                return;
            if (itemList.Count == 0)
                return;

            GameObject prefab = eq_left.inventory.slotTemplate;
            Transform p = eq_left.inventory.slotGrid;

            int dif = iconSlotsCreated.Count - itemList.Count;
            int extra = (dif > 0) ? dif : 0;

            for (int i = 0; i < itemList.Count + extra; i++)
            {
                if (i > itemList.Count - 1)
                {
                    iconSlotsCreated[i].gameObject.SetActive(false);
                    continue;
                }

                IconBase icon = null;
                if (iconSlotsCreated.Count - 1 < i)
                {
                    GameObject g = Instantiate(prefab) as GameObject;
                    g.SetActive(true);
                    g.transform.SetParent(p);
                    icon = g.GetComponent<IconBase>();
                    iconSlotsCreated.Add(icon);
                }
                else
                {
                    icon = iconSlotsCreated[i];
                }
                icon.gameObject.SetActive(true);
                icon.icon.enabled = true;
                icon.icon.sprite = itemList[i].icon;
                icon.id = itemList[i].Item_id;
            }
        }

        public void Tick()
        {

        }
        public void LoadEquipment(InventoryManager inv)
        {
            for (int i = 0; i < inv.rh_weapons.Count; i++)
            {
                if (i > 2)
                    break;

                EquipmentSlot slot = equipmentSlotUI.weaponSlots[i];
                equipmentSlotUI.UpdateSlot(inv.rh_weapons[i], slot, ItemType.weapon);
            }
            for (int i = 0; i < inv.lh_weapons.Count; i++)
            {
                if (i > 2)
                    break;

                EquipmentSlot slot = equipmentSlotUI.weaponSlots[i + 3];
                equipmentSlotUI.UpdateSlot(inv.lh_weapons[i], slot, ItemType.weapon);
            }
            for (int i = 0; i < inv.consumable_items.Count; i++)
            {
                if (i > 9)
                    break;

                EquipmentSlot slot = equipmentSlotUI.consumableSlots[i];
                equipmentSlotUI.UpdateSlot(inv.consumable_items[i], slot, ItemType.consumable);
            }

        }
        public InventoryManager invManager;
        public bool load;

        void Update()
        {
            if (load)
            {
                LoadEquipment(invManager);
                prevEqSlot = null;
                load = false;
            }
            HandleSlotMovement();

            if (prevEqSlot != null)
            {
                LoadItemFromSlot();
            }
            prevEqSlot = curEqSlot;
        }
        void LoadItemFromSlot()
        {
            if (curEqSlot == null)
                return;

            if (string.IsNullOrEmpty(curEqSlot.icon.id))
            {
                curEqSlot.icon.id = "Unarmed";
            }
            eq_left.slotName.text = curEqSlot.slotName;

            ResourcesManager rm = ResourcesManager.singleton;



            switch (curEqSlot.slotType)
            {
                case EqSlotType.weapons:
                    LoadWeaponItem(rm);
                    break;
                case EqSlotType.equipment:
                    break;
                case EqSlotType.consumables:
                    LoadConsumableItem(rm);
                    break;
                case EqSlotType.rings:
                    break;
                case EqSlotType.covenant:
                    break;
                default:
                    break;
            }
        }
        void LoadWeaponItem(ResourcesManager rm)
        {
            string weaponId = curEqSlot.icon.id;
            WeaponStats stats = rm.GetWeaponStats(weaponId);
            Item item = rm.GetItem(curEqSlot.icon.id, ItemType.weapon);
            eq_left.curItemName.text = item.name_item;
            UpdateCenterOverlay(item);
            //center main
            weaponInfo.smallIcon.sprite = item.icon;
            weaponInfo.itemName.text = item.name_item;
            weaponInfo.weaponType.text = stats.weaponType;
            weaponInfo.damageType.text = stats.weaponDamageType;
            weaponInfo.skillName.text = stats.skillName;
            weaponInfo.weightCost.text = stats.weightCost.ToString();

            weaponInfo.durability_min.text = stats.maxDurability.ToString();
            weaponInfo.durability_max.text = stats.maxDurability.ToString();

            c_overlay.skillName.text = stats.skillName;

            UpdateUIAttackElements(AttackDefenseType.physical, weaponInfo.ap_slots, stats.a_physical.ToString());
            UpdateUIAttackElements(AttackDefenseType.magic, weaponInfo.ap_slots, stats.a_magic.ToString());
            UpdateUIAttackElements(AttackDefenseType.fire, weaponInfo.ap_slots, stats.a_fire.ToString());
            UpdateUIAttackElements(AttackDefenseType.lightning, weaponInfo.ap_slots, stats.a_lightning.ToString());
            UpdateUIAttackElements(AttackDefenseType.dark, weaponInfo.ap_slots, stats.a_dark.ToString());
            UpdateUIAttackElements(AttackDefenseType.critical, weaponInfo.ap_slots, stats.critical.ToString());


            UpdateUIAttackElements(AttackDefenseType.frost, weaponInfo.a_effects, stats.a_frost.ToString(), true);
            // UpdateUIAttackElements(AttackDefenseType.poison,weaponInfo.a_effects_Grid, stats.poison.ToString());
            UpdateUIAttackElements(AttackDefenseType.curse, weaponInfo.a_effects, stats.a_curse.ToString(), true);
            //  UpdateUIAttackElements(AttackDefenseType.strike,weaponInfo.a_effects_Grid, stats.strike.ToString());
            // UpdateUIAttackElements(AttackDefenseType.slash,weaponInfo.a_effects_Grid, stats.slash.ToString());
            // UpdateUIAttackElements(AttackDefenseType.thrust,weaponInfo.a_effects_Grid, stats.thrust.ToString());


            UpdateUIAttackElements(AttackDefenseType.physical, weaponInfo.g_absorb, stats.d_physical.ToString());
            UpdateUIAttackElements(AttackDefenseType.magic, weaponInfo.g_absorb, stats.d_magic.ToString());
            UpdateUIAttackElements(AttackDefenseType.fire, weaponInfo.g_absorb, stats.d_fire.ToString());
            UpdateUIAttackElements(AttackDefenseType.lightning, weaponInfo.g_absorb, stats.d_lightning.ToString());
            UpdateUIAttackElements(AttackDefenseType.dark, weaponInfo.g_absorb, stats.d_dark.ToString());
            UpdateUIAttackElements(AttackDefenseType.stability, weaponInfo.g_absorb, stats.stability.ToString());

        }
        void UpdateUIAttackElements(AttackDefenseType t, List<AttDefType> l, string value, bool onTxt1 = false)
        {
            AttDefType s1 = weaponInfo.GetAttDefSlot(l, t);
            if (s1?.slot == null) return;

            if (!onTxt1)
                s1.slot.txt2.text = value;
            else
                s1.slot.txt1.text = value;
        }
        void UpdateCenterOverlay(Item item)
        {
            //center overlay
            c_overlay.bigIcon.sprite = item.icon;

            c_overlay.itemName.text = item.name_item;
            c_overlay.itemDescription.text = item.itemDescription;
            c_overlay.skillName.text = item.skillDescription;
            c_overlay.skillDescription.text = item.skillDescription;
      
        }
        void LoadConsumableItem(ResourcesManager rm)
        {
            string weaponId = curEqSlot.icon.id;
            Item item = rm.GetItem(curEqSlot.icon.id, ItemType.consumable);

            UpdateCenterOverlay(item);

        }

        public static InventoryUI singleton;

        public void Awake()
        {
            singleton = this;
        }

        [System.Serializable]
        public class EquipmentSlotUI
        {
            public List<EquipmentSlot> weaponSlots = new List<EquipmentSlot>();
            public List<EquipmentSlot> arrowSlots = new List<EquipmentSlot>();
            public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
            public List<EquipmentSlot> ringSlots = new List<EquipmentSlot>();
            public List<EquipmentSlot> consumableSlots = new List<EquipmentSlot>();
            public EquipmentSlot covenantSlot;

            public void UpdateSlot(string itemId, EquipmentSlot s, ItemType itemType)
            {
                Item item = ResourcesManager.singleton.GetItem(itemId, itemType);
                s.icon.icon.sprite = item.icon;
                s.icon.icon.enabled = true;
                s.icon.id = item.Item_id;
            }
            public void AddSlotOnList(EquipmentSlot eq)
            {
                switch (eq.slotType)
                {
                    case EqSlotType.weapons:
                        weaponSlots.Add(eq);
                        break;
                    case EqSlotType.arrows:
                    case EqSlotType.bolts:
                        arrowSlots.Add(eq);
                        break;
                    case EqSlotType.equipment:
                        equipmentSlots.Add(eq);
                        break;
                    case EqSlotType.rings:
                        ringSlots.Add(eq);
                        break;
                    case EqSlotType.covenant:
                        covenantSlot = eq;
                        break;
                    case EqSlotType.consumables:
                        consumableSlots.Add(eq);
                        break;
                    default:
                        break;
                }
            }
        }


        [System.Serializable]
        public class EquipmentLeft
        {
            public TMP_Text slotName;
            public TMP_Text curItemName;
            public Left_Inventory inventory;

        }
        [System.Serializable]
        public class PlayerStatus
        {
            public GameObject slot_template;
            public GameObject doubleSlot_template;
            public GameObject emptySlot;
            public Transform attGrid;
            public Transform defGrid;
            public Transform apGrid;
            public Transform resistGrid;
            public List<AttributeSlot> attSlots = new List<AttributeSlot>();
            public List<AttackPowerSlot> apSlots = new List<AttackPowerSlot>();
            public List<PlayerStatusDef> defSlots = new List<PlayerStatusDef>();
            public List<PlayerStatusDef> resistSlots = new List<PlayerStatusDef>();
        }
        [System.Serializable]
        public class Left_Inventory
        {
            public Slider inventorySlider;
            public GameObject slotTemplate;
            public Transform slotGrid;
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
        public class AttDefType
        {
            public bool isBreak;
            public AttackDefenseType type;
            public InventoryUISlot slot;
        }

        [System.Serializable]
        public class AttackPowerSlot
        {
            public InventoryUISlot slot;
        }


        [System.Serializable]
        public class PlayerStatusDef
        {
            public AttackDefenseType type;
            public InventoryUIDoubleSlot slot;
        }

    }
    public enum EqSlotType
    {
        weapons, arrows, bolts, equipment, rings, covenant, consumables
    }
    public enum UIState
    {
        eqipment, inventory, attributes, messages, options
    }
}
