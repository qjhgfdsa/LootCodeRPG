using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class PickableItemsManager : MonoBehaviour
    {
        public List<PickableItem> pick_items = new List<PickableItem>();
        public PickableItem itemCanidate;

        int frameCount;
        public int frameCheck = 15;

        public void Tick()
        {
            if (frameCount < frameCheck)
            {
                frameCount++;
                return;
            }
            frameCount = 0;

            for (int i = 0; i < pick_items.Count; i++)
            {
                float d = Vector3.Distance(pick_items[i].transform.position, transform.position);
                if (d < 1.5f)
                {
                    itemCanidate = pick_items[i];

                }
                else
                {
                    if (itemCanidate == pick_items[i])
                        itemCanidate = null;
                }
            }
        }
        public void PickCanidate()
        {
            if (itemCanidate == null)
                return;

            SessionManager s = SessionManager.singleton;

            for (int i = 0; i < itemCanidate.items.Length; i++)
            {
                PickItemContainer c = itemCanidate.items[i];
                s.AddItem(c.itemId, c.itemType);
            }
            if (pick_items.Contains(itemCanidate))
                pick_items.Remove(itemCanidate);
                
            Destroy(itemCanidate.gameObject);
            itemCanidate = null;
        }
    }
}
