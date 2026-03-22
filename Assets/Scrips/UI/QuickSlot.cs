using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA.UI
{
    public class QuickSlot : MonoBehaviour
    {
        public List<QSlots> slots = new List<QSlots>();

        public void Init()
        {
          ClearIcons();
        }

        public void ClearIcons()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].icon != null)
                    slots[i].icon.gameObject.SetActive(false);
            }
        }

        public void UpdateSlot(QSlotType stype, Sprite i)
        {
            QSlots q = GetSlot(stype);
            if (q == null)
            {
                Debug.LogWarning(
                    $"QuickSlot: No slot for '{stype}'. Add a QSlots entry with this type and assign its Image in the Inspector.");
                return;
            }

            if (q.icon == null)
            {
                Debug.LogWarning($"QuickSlot: Slot '{stype}' has no Image assigned.");
                return;
            }

            q.icon.sprite = i;
            q.icon.gameObject.SetActive(true);
        }
        
        public QSlots GetSlot(QSlotType t)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].type == t)
                    return slots[i];
            }

            return null;
            
        }
        
        public static QuickSlot singleton;
        void Awake()
        {
            singleton = this;
        }
        




    }

    public enum QSlotType

    {
        rh, lh, item, spell
    }

    [System.Serializable]
    public class QSlots
    {
        public QSlotType type;
        public Image icon;
        //public Text amount;
    }
}
