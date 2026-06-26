using UnityEngine;
namespace SA
{
    public class NPCInteraction : WorldInteraction
    {
        public string npc_id;
        public override void InteractActual()
        {
            DialogueManager.singleton.InitDialogue(this.transform, npc_id);
        }
    }
}
