using UnityEngine;

namespace SA
{
    public class NPCScriptableObject : ScriptableObject
    {
        public NPCDialogue[] npc;
    }
    [System.Serializable]
    public class NPCDialogue
    {
        public string npc_id;
        public Dialogue[] dialogues;
    }
    [System.Serializable]
    public class Dialogue
    {
        public string[] dialogText;
        public string specialEvent;
        public bool increaseIndex;
        public string targetAnim;
        public bool playAnim;
    }
}
