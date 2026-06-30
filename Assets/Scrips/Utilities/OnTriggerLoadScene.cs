using UnityEngine;

namespace SA
{
    public class OnTriggerLoadScene : MonoBehaviour
    {
        public string loadLevel;
        public string unloadLevel;

        const string LogTag = "[OnTriggerLoadScene]";

        public void OnTriggerEnter(Collider other)
        {
            InputHandler inp = other.GetComponent<InputHandler>();
            if (inp == null)
                inp = other.GetComponentInParent<InputHandler>();

            if (inp == null)
            {
                Debug.Log($"{LogTag} Trigger '{name}' ignored — no InputHandler on '{other.name}' (tag={other.tag}).");
                return;
            }

            Debug.Log($"{LogTag} Trigger '{name}' by player '{other.name}' load='{loadLevel}' unload='{unloadLevel}'");

            if (MySceneManager.singleton == null)
            {
                Debug.LogError($"{LogTag} MySceneManager.singleton is null.");
                return;
            }

            if (string.IsNullOrEmpty(loadLevel) == false)
                MySceneManager.singleton.LoadScene(loadLevel);

            if (string.IsNullOrEmpty(unloadLevel) == false)
                MySceneManager.singleton.UnloadScene(unloadLevel);
        }
    }
}
