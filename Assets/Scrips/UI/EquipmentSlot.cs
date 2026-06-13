using UnityEngine;  
using SA.UI;
using System.Collections.Generic;
using System.Collections;
namespace SA.UI
{
    public class EquipmentSlot : MonoBehaviour
    {
        public string slotName;
        public IconBase icon;
        public EqSlotType slotType;
        public Vector2 slotPos;

        public void Init( InventoryUI ui)
        {
            icon = GetComponent<IconBase>();
            ui.equipmentSlotUI.AddSlotOnList(this);
        }

    }
}
