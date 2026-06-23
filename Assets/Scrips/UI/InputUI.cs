using UnityEngine;

namespace SA
{
    public class InputUI : MonoBehaviour
    {
        public float vertical;
        public float horizontal;
        public bool key1_input, key2_input, key3_input, t_input;

        public void Tick()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);

            key1_input = Input.GetKeyDown(KeyCode.Alpha1);
            key2_input = Input.GetKeyDown(KeyCode.Alpha2);
            key3_input = Input.GetKeyDown(KeyCode.Alpha3);

            t_input = Input.GetKeyDown(StaticStrings.KeyT);
        }
        public static InputUI singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
