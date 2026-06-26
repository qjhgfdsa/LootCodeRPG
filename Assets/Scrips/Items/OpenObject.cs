using UnityEngine;

namespace SA
{
    public class OpenObject : WorldInteraction
    {
        public GameObject obj;
        public override void InteractActual()
        {
            obj.SetActive(true);
            base.InteractActual();
        }

    }
}
