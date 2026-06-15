using System.Collections.Generic;
using SA;
using SA.UI.Icons;
using UnityEngine;
using UnityEngine.UI;

namespace SA.UI
{
    public class QuickSlot : MonoBehaviour
    {
        const string DefaultProfilePath = "SA.QuickSlotIconProfile";

        public IconDisplayProfile slotProfile;
        public List<QSlots> slots = new List<QSlots>();

        public void Init()
        {
            EnsureProfile();
            ClearIcons();
        }

        public void ClearIcons()
        {
            EnsureProfile();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].icon != null)
                    IconPresenter.Clear(slots[i].icon);
            }
        }

        public void ClearSlot(QSlotType stype)
        {
            QSlots q = GetSlot(stype);
            q.icon.gameObject.SetActive(false);
        }
        public void UpdateSlot(QSlotType stype, IconId iconId)
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

            EnsureProfile();

            if (iconId.IsEmpty)
            {
                IconPresenter.Clear(q.icon);
                return;
            }

            IconPresenter.Show(q.icon, iconId, slotProfile);
        }

        void EnsureProfile()
        {
            if (slotProfile == null)
                slotProfile = Resources.Load<IconDisplayProfile>(DefaultProfilePath);
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
            EnsureProfile();
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
    }
}
