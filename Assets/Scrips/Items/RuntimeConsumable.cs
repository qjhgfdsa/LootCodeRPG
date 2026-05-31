using UnityEngine;

namespace SA
{
    public class RuntimeConsumable : MonoBehaviour
    {
        public int itemCount = 2;
        public bool unlimitedCount;
        public Consumable instance;
        public GameObject itemModel;
    }
}
