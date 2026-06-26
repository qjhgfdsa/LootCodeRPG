using UnityEngine;
using System.Collections.Generic;

namespace SA
{
    public class InteractionsScriptableObject : ScriptableObject
    {
        public Interactions[] interactions;

    }
    [System.Serializable]
    public class Interactions
    {
        public string InteractionId;
        public string anim;
        public bool oneShot;
        public string specialEvent;

    }
}
