using UnityEngine;

namespace SA
{
    public class PickableItem : MonoBehaviour
    {
       public PickItemContainer[] items;
    }
    [System.Serializable]
    public class PickItemContainer
    {
        public string itemId;
        public ItemType itemType;
       
    }

}
