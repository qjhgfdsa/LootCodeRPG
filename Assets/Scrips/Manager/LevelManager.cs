using UnityEngine;

namespace SA
{
    public class LevelManager : MonoBehaviour
    {
        public Transform spawnPosition;
        public WorldInteraction[] worldInteractions;

        public static LevelManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
