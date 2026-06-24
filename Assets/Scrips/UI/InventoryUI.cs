using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using SA.UI.Icons;



namespace SA.UI
{
    public class InventoryUI : MonoBehaviour
    {
        const string DefaultInventoryIconProfilePath = "SA.InventoryIconProfile";

        public IconDisplayProfile inventoryIconProfile;
        public EquipmentLeft eq_left;
        public CenterOverlay c_overlay;
        public WeaponInfo weaponInfo;
        public PlayerStatus playerStatus;


        public GameObject gameMenu, inventoryMenu, centerMain, centerRight, centerOverlay;
        public GameObject equipmentScreen;
        public GameObject inventoryScreen;
        public GameObject gameUI;

        List<IconBase> iconSlotsCreated = new List<IconBase>();
        public EquipmentSlotUI equipmentSlotUI;

        public Transform equipmentParent;
        // List<EquipmentSlot> equipSlots = new List<EquipmentSlot>();
        EquipmentSlot[,] equipSlots;
        public Vector2 curSlotPos;
        public int curInvIndex;
        List<IconBase> curCreatedItems;
        IconBase curInvIcon;
        int prevInvIndex;
        int maxInvIndex;

        float inputT;
        bool dontMove;

        public Color unselected;
        public Color selected;
        EquipmentSlot curEqSlot;
        EquipmentSlot prevEqSlot;
        float inpTimer;
        float moveTimer = 0.4f;

        InputUI inp;
        InventoryManager invManager;
        public bool isSwitching;
        public bool isMenu;
        bool centerOverlayIsActive;

        SessionManager session;

        #region ปุ่มเลือกออบเจ็ค ใน inventory

        void ClearOnIndex(int ind)
        {
            int ix = ind;
            if (ix > 2)
            {
                ix -= 3;
                invManager.lh_weapons[ix] = -1;
            }
            else
            {
                invManager.rh_weapons[ix] = -1;
            }
        }
        void HandleSlotInput(InputUI inp)
        {
            if (curEqSlot == null)
                return;

            if (inp.key1_input)//เดะเเก้
            {
                isSwitching = !isSwitching;
                if (isSwitching)
                {
                    CloseCreatedItems();
                    ItemType t = ItemTypeFromSlotType(curEqSlot.slotType);
                    LoadCurrentItems(t);
                }
                else
                {
                    if (curInvIcon != null)
                    {
                        if (curInvIcon.icon.isActiveAndEnabled)
                        {
                            ItemType t = ItemTypeFromSlotType(curEqSlot.slotType);

                            switch (t)
                            {
                                case ItemType.weapon:
                                    changeWeapon();
                                    break;
                                case ItemType.consumable:
                                    changeConsumable();
                                    break;
                                case ItemType.equipment:
                                    changeArmor();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    LoadEquipment(invManager, true);
                }
                ChangeToSwitching();
            }
            if (inp.key3_input)
            {
                if (isSwitching)
                {
                    isSwitching = false;
                    ChangeToSwitching();
                }
                else
                {
                    isMenu = false;
                    CloseUI();
                }
            }
            if (inp.key2_input) //remove item from slot
            {
                if (isSwitching)
                {
                    centerOverlayIsActive = !centerOverlayIsActive;
                    centerOverlay.SetActive(centerOverlayIsActive);
                }
                else
                {
                    ItemType t = ItemTypeFromSlotType(curEqSlot.slotType);

                    switch (t)
                    {
                        case ItemType.weapon:
                            clearWeapon();
                            break;
                        case ItemType.consumable:
                            clearConsumable();
                            break;
                        case ItemType.equipment:
                            clearArmor();
                            break;
                        default:
                            break;
                    }
                    LoadEquipment(invManager, true);
                }
            }
        }
        #endregion
        void clearWeapon()
        {
            int targetIndex = curEqSlot.itemPosition;
            bool isLeft = (curEqSlot.itemPosition > 2) ? true : false;

            if (isLeft)
            {
                targetIndex -= 3;
                invManager.lh_weapons[targetIndex] = -1;

                ItemInventoryInstance inst = session.GetWeaponItem(invManager.lh_weapons[targetIndex]);
                if (inst.slot != null)
                {
                    equipmentSlotUI.ClearEqSlot(inst.slot, ItemType.weapon);
                }
            }
            else
            {
                invManager.rh_weapons[targetIndex] = -1;
                ItemInventoryInstance inst = session.GetWeaponItem(invManager.rh_weapons[targetIndex]);

                if (inst.slot != null)
                {
                    equipmentSlotUI.ClearEqSlot(inst.slot, ItemType.weapon);
                }
            }
        }
        void clearConsumable()
        {
            int targetIndex = curEqSlot.itemPosition;
            if (targetIndex < invManager.consumable_items.Count)
            {
                invManager.consumable_items[curEqSlot.itemPosition] = -1;

                ItemInventoryInstance inst = session.GetWeaponItem(invManager.consumable_items[targetIndex]);
                if (inst.slot != null)
                {
                    equipmentSlotUI.ClearEqSlot(inst.slot, ItemType.consumable);
                }
            }
        }
        void clearArmor()
        {
            switch (curEqSlot.armorType)
            {
                case ArmorType.head:
                    invManager.armorManager.headId = -1;
                    break;
                case ArmorType.chest:
                    invManager.armorManager.chestId = -1;
                    break;
                case ArmorType.legs:
                    invManager.armorManager.legsId = -1;
                    break;
                case ArmorType.hands:
                    invManager.armorManager.handsId = -1;
                    break;
                default:
                    break;
            }
        }
        void changeWeapon()
        {
            int targetIndex = curEqSlot.itemPosition;
            bool isLeft = (curEqSlot.itemPosition > 2) ? true : false;

            if (isLeft)
            {
                if (curInvIcon == null)
                    return;

                targetIndex -= 3;
                invManager.lh_weapons[targetIndex] = curInvIcon.id;

                ItemInventoryInstance inst = session.GetWeaponItem(invManager.lh_weapons[targetIndex]);
                if (inst.slot != null)
                {
                    equipmentSlotUI.ClearEqSlot(inst.slot, ItemType.weapon);
                    ClearOnIndex(inst.eq_index);
                }
            }
            else
            {
                invManager.rh_weapons[targetIndex] = curInvIcon.id;

                ItemInventoryInstance inst = session.GetWeaponItem(invManager.rh_weapons[targetIndex]);
                if (inst.slot != null)
                {
                    equipmentSlotUI.ClearEqSlot(inst.slot, ItemType.weapon);
                    ClearOnIndex(inst.eq_index);
                }
            }

        }
        void changeConsumable()
        {
            ItemInventoryInstance inst = session.GetConItem(curInvIcon.id);
            if (inst.slot != null)
            {
                equipmentSlotUI.ClearEqSlot(inst.slot, ItemType.consumable);
                invManager.consumable_items[inst.eq_index] = -1;
            }

            invManager.consumable_items[curEqSlot.itemPosition] = curInvIcon.id;
        }
        void changeArmor()
        {
            if (curInvIcon == null)
            {
                return;
            }

            switch (curEqSlot.armorType)
            {
                case ArmorType.head:
                    invManager.armorManager.headId = curInvIcon.id;
                    break;
                case ArmorType.chest:
                    invManager.armorManager.chestId = curInvIcon.id;
                    break;
                case ArmorType.legs:
                    invManager.armorManager.legsId = curInvIcon.id;
                    break;
                case ArmorType.hands:
                    invManager.armorManager.handsId = curInvIcon.id;
                    break;
                default:
                    break;
            }
        }
        ItemType ItemTypeFromSlotType(EqSlotType t)
        {
            switch (t)
            {
                case EqSlotType.weapons:
                    return ItemType.weapon;
                case EqSlotType.arrows:
                case EqSlotType.bolts:
                case EqSlotType.equipment:
                    return ItemType.equipment;
                case EqSlotType.rings:
                case EqSlotType.covenant:
                default:
                    return ItemType.spell;
                case EqSlotType.consumables:
                    return ItemType.consumable;
            }
        }
        void HandleSlotMovement(InputUI inp)
        {
            int x = Mathf.RoundToInt(curSlotPos.x);
            int y = Mathf.RoundToInt(curSlotPos.y);


            bool up = (inp.vertical > 0);
            bool down = (inp.vertical < 0);
            bool left = (inp.horizontal < 0);
            bool right = (inp.horizontal > 0);

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
        void HandleInventoryMovement(InputUI inp)
        {

            bool up = (inp.vertical > 0);
            bool down = (inp.vertical < 0);
            bool left = (inp.horizontal < 0);
            bool right = (inp.horizontal > 0);

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
                curInvIndex -= 5;
                inpTimer = moveTimer;
            }
            if (down)
            {
                curInvIndex += 5;
                inpTimer = moveTimer;
            }
            if (left)
            {
                curInvIndex -= 1;
                inpTimer = moveTimer;
            }
            if (right)
            {
                curInvIndex += 1;
                inpTimer = moveTimer;
            }

            if (curInvIndex > maxInvIndex - 1)
                curInvIndex = 0;

            if (curInvIndex < 0)
                curInvIndex = maxInvIndex - 1;

        }

        #region Create UI Elements
        public void Pre_Init()
        {
            session = SessionManager.singleton;

            CreateUIElements();
            InitEqSlots();
        }
        public void Init(InventoryManager inv)
        {
            inp = InputUI.singleton;
            invManager = inv;
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
        public void CloseCreatedItems()
        {
            for (int i = 0; i < iconSlotsCreated.Count; i++)
            {
                iconSlotsCreated[i].gameObject.SetActive(false);
            }
        }
        public void LoadCurrentItems(ItemType t)
        {
            //List<Item> itemList = session.GetItemAsList(t);
            List<ItemInventoryInstance> itemList = session.GetItemsIntanceList(t);

            if (itemList == null)
                return;

            List<ItemInventoryInstance> candidateList = new List<ItemInventoryInstance>();

            if (t == ItemType.equipment)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    ArmorContainer armor = ResourcesManager.singleton.GetArmor(itemList[i].itemId);

                    if (armor.armorType == curEqSlot.armorType)
                    {
                        candidateList.Add(itemList[i]);
                    }
                }
            }
            else
            {
                candidateList.AddRange(itemList);
            }

            if (candidateList.Count == 0)
                return;

            GameObject prefab = eq_left.inventory.slotTemplate;
            Transform p = eq_left.inventory.slotGrid;

            int dif = iconSlotsCreated.Count - candidateList.Count;
            int extra = (dif > 0) ? dif : 0;

            maxInvIndex = candidateList.Count;

            curCreatedItems = new List<IconBase>();
            curInvIndex = 0;

            for (int i = 0; i < candidateList.Count + extra; i++)
            {
                if (i > candidateList.Count - 1)
                {
                    iconSlotsCreated[i].gameObject.SetActive(false);
                    continue;
                }

                Item item = ResourcesManager.singleton.GetItem(candidateList[i].itemId, t);

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

                curCreatedItems.Add(icon);
                icon.gameObject.SetActive(true);
                IconPresenter.Show(icon.icon, item.icon, InventoryIconProfile);
                icon.id = candidateList[i].uniqueId;
            }
        }
        void ChangeToSwitching()
        {
            if (isSwitching)
            {
                curInvIndex = 0;
                prevInvIndex = -1;

                equipmentScreen.SetActive(false);
                inventoryScreen.SetActive(true);
            }
            else
            {
                equipmentScreen.SetActive(true);
                inventoryScreen.SetActive(false);
            }

        }
        void HandleUIState(InputUI inp)
        {
            switch (curUIState)
            {
                case UIState.eqipment:
                    if (!isSwitching)
                        HandleSlotMovement(inp);
                    else
                        HandleInventoryMovement(inp);

                    HandleSlotInput(inp);
                    break;
                case UIState.inventory:
                    HandleInventoryMovement(inp);
                    break;
                case UIState.attributes:
                    break;
                case UIState.messages:
                    break;
                case UIState.options:
                    break;
                default:
                    break;
            }
        }
        public void OpenUI()
        {
            LoadEquipment(invManager);
            gameMenu.SetActive(false);
            inventoryMenu.SetActive(true);
            gameUI.SetActive(false);
            prevEqSlot = null;
            curInvIndex = 0;
            prevInvIndex = -1;
            isSwitching = false;

            if (equipSlots != null && equipSlots[0, 0] != null)
                SelectEqSlot(equipSlots[0, 0]);
            else if (equipmentSlotUI.weaponSlots.Count > 0)
                SelectEqSlot(equipmentSlotUI.weaponSlots[0]);

            if (curEqSlot != null)
            {
                eq_left.slotName.text = curEqSlot.slotName;
                LoadItemFromSlot(curEqSlot.icon);
            }
        }
        void SelectEqSlot(EquipmentSlot slot)
        {
            if (slot == null)
                return;

            if (curEqSlot != null)
                curEqSlot.icon.background.color = unselected;

            curEqSlot = slot;
            curSlotPos = curEqSlot.slotPos;
            curEqSlot.icon.background.color = selected;
        }
        public void CloseUI()
        {
            gameMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            gameUI.SetActive(true);
            prevEqSlot = null;
            curInvIndex = -1;
        }
        public void Tick()
        {
            inp.Tick();
            HandleUIState(inp);

            if (inp.t_input)
            {
                centerRight.SetActive(!centerRight.activeInHierarchy);
            }

            if (isSwitching)
            {
                if (curCreatedItems != null && curCreatedItems.Count > 0 && prevInvIndex != curInvIndex)
                {
                    if (curInvIcon)
                        curInvIcon.background.color = unselected;

                    if (curInvIndex < curCreatedItems.Count)
                    {
                        curInvIcon = curCreatedItems[curInvIndex];
                        curInvIcon.background.color = selected;
                        LoadItemFromSlot(curInvIcon);
                    }
                }
            }
            else if (curEqSlot != null && curEqSlot != prevEqSlot)
            {
                eq_left.slotName.text = curEqSlot.slotName;
                LoadItemFromSlot(curEqSlot.icon);
            }

            prevEqSlot = curEqSlot;
            prevInvIndex = curInvIndex;
        }
        public void LoadEquipment(InventoryManager inv, bool loadOnCharacter = false)
        {
            if (loadOnCharacter)
                inv.ClearReferences();

            for (int i = 0; i < inv.rh_weapons.Count; i++)
            {
                if (i > 2)
                    break;

                EquipmentSlot slot = equipmentSlotUI.weaponSlots[i];

                if (inv.rh_weapons[i] == -1)
                {
                    equipmentSlotUI.ClearEqSlot(slot, ItemType.weapon);
                }
                else
                {
                    ItemInventoryInstance inst = session.GetWeaponItem(inv.rh_weapons[i]);
                    inst.slot = slot;
                    inst.eq_index = i;

                    equipmentSlotUI.UpdateSlot(inst.uniqueId, slot, ItemType.weapon);
                }
            }

            for (int i = 0; i < inv.lh_weapons.Count; i++)
            {
                if (i > 2)
                    break;
                EquipmentSlot slot = equipmentSlotUI.weaponSlots[i + 3];
                if (inv.lh_weapons[i] == -1)
                {
                    equipmentSlotUI.ClearEqSlot(slot, ItemType.weapon);
                }
                else
                {
                    ItemInventoryInstance inst = session.GetWeaponItem(inv.lh_weapons[i]);
                    inst.slot = slot;
                    inst.eq_index = i + 3;

                    equipmentSlotUI.UpdateSlot(inst.uniqueId, slot, ItemType.weapon);
                }
            }

            for (int i = 0; i < inv.consumable_items.Count; i++)
            {
                if (i > 9)
                    break;

                EquipmentSlot slot = equipmentSlotUI.consumableSlots[i];
                if (inv.consumable_items[i] == -1)
                {
                    equipmentSlotUI.ClearEqSlot(slot, ItemType.consumable);
                }
                else
                {
                    ItemInventoryInstance inst = session.GetConItem(inv.consumable_items[i]);
                    inst.slot = slot;
                    inst.eq_index = i;

                    equipmentSlotUI.UpdateSlot(inst.uniqueId, slot, ItemType.consumable);
                }

            }
            LoadArmor();

            if (loadOnCharacter)
            {
                invManager.LoadInventory(true);
            }

        }

        void LoadArmor()
        {
            UpdateArmorSlot(equipmentSlotUI.equipmentSlots[0], ArmorType.head, invManager.armorManager.headId);
            UpdateArmorSlot(equipmentSlotUI.equipmentSlots[1], ArmorType.chest, invManager.armorManager.chestId);
            UpdateArmorSlot(equipmentSlotUI.equipmentSlots[2], ArmorType.legs, invManager.armorManager.legsId);
            UpdateArmorSlot(equipmentSlotUI.equipmentSlots[3], ArmorType.hands, invManager.armorManager.handsId);
        }
        void UpdateArmorSlot(EquipmentSlot slot, ArmorType t, int id)
        {
            if (id == -1)
            {
                equipmentSlotUI.ClearEqSlot(slot, ItemType.equipment);
                return;
            }
            ItemInventoryInstance inst = session.GetArmorItem(id);
            inst.slot = slot;
            equipmentSlotUI.UpdateSlot(id, slot, ItemType.equipment);
        }
        void LoadItemFromSlot(IconBase icon)
        {
            // eq_left.slotName.text = curEqSlot.slotName;

            ResourcesManager rm = ResourcesManager.singleton;

            switch (curEqSlot.slotType)
            {
                case EqSlotType.weapons:
                    LoadWeaponItem(rm, icon);
                    break;
                case EqSlotType.equipment:
                    UpdateItemSlotInfo(rm, icon, ItemType.equipment);
                    break;
                case EqSlotType.consumables:
                    UpdateItemSlotInfo(rm, icon, ItemType.consumable);
                    break;
                case EqSlotType.rings:
                    break;
                case EqSlotType.covenant:
                    break;
                default:
                    break;
            }
        }
        void LoadWeaponItem(ResourcesManager rm, IconBase icon)
        {
            ItemInventoryInstance inst = session.GetWeaponItem(icon.id);

            string weaponId = inst.itemId;
            WeaponStats stats = rm.GetWeaponStats(weaponId);
            Item item = rm.GetItem(weaponId, ItemType.weapon);
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


        void UpdateItemSlotInfo(ResourcesManager rm, IconBase icon, ItemType t)
        {
            ItemInventoryInstance inst = null;
            if (centerOverlayIsActive == false)
                centerOverlay.SetActive(false);

            switch (t)
            {
                case ItemType.consumable:
                    inst = session.GetConItem(icon.id);
                    centerOverlay.SetActive(true);
                    break;
                case ItemType.equipment:
                    inst = session.GetArmorItem(icon.id);
                    centerOverlay.SetActive(true);
                    break;
                default:
                    break;
            }

            string itemId = inst.itemId;
            Item item = rm.GetItem(itemId, t);
            UpdateCenterOverlay(item);
            eq_left.curItemName.text = item.name_item;
        }

        public static InventoryUI singleton;

        public IconDisplayProfile InventoryIconProfile
        {
            get
            {
                EnsureIconProfile();
                return inventoryIconProfile;
            }
        }

        void EnsureIconProfile()
        {
            if (inventoryIconProfile == null)
                inventoryIconProfile = Resources.Load<IconDisplayProfile>(DefaultInventoryIconProfilePath);
        }

        public void Awake()
        {
            singleton = this;
            EnsureIconProfile();
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

            public void ClearEqSlot(EquipmentSlot s, ItemType itemType)
            {
                IconPresenter.Clear(s.icon.icon);
                s.icon.id = -1;
            }
            public void UpdateSlot(int uniqueId, EquipmentSlot s, ItemType itemType)
            {
                ItemInventoryInstance inst = null;

                switch (itemType)
                {
                    case ItemType.weapon:
                        inst = SessionManager.singleton.GetWeaponItem(uniqueId);
                        break;
                    case ItemType.spell:
                        break;
                    case ItemType.consumable:
                        inst = SessionManager.singleton.GetConItem(uniqueId);
                        break;
                    case ItemType.equipment:
                        inst = SessionManager.singleton.GetArmorItem(uniqueId);
                        break;
                    default:
                        break;
                }
                if (inst == null)
                    return;

                Item item = ResourcesManager.singleton.GetItem(inst.itemId, itemType);
                IconPresenter.Show(s.icon.icon, item.icon, InventoryUI.singleton.InventoryIconProfile);
                s.icon.id = uniqueId;
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
            public EquipmentSlot GetWeaponSlot(int index)
            {
                return weaponSlots[index];
            }
            public EquipmentSlot GetConSlot(int index)
            {
                return consumableSlots[index];
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
