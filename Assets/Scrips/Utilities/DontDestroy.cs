using UnityEngine;

namespace SA
{
    public class DontDestroy : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
