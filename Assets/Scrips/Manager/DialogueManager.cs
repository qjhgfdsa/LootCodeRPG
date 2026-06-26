using UnityEngine;
using TMPro;

namespace SA
{
    public class DialogueManager : MonoBehaviour
    {
        public TextMeshProUGUI dialogueText;
        public GameObject textObj;
        Transform origin;
        NPCDialogue npc_d;
        NPCStates npc_state;
        public bool dialogueActive;
        bool updateDialogue;
        int lineIndex;

        public void InitDialogue(Transform o, string id)
        {
            origin = o;
            npc_d = ResourcesManager.singleton.GetDialogue(id);
            npc_state = SessionManager.singleton.GetNPCState(id);
            dialogueActive = true;
            textObj.SetActive(true);
            updateDialogue = false;
            lineIndex = 0;
        }
        public void Tick(bool a_input)
        {
            if (!dialogueActive)
                return;

            if (!updateDialogue)
            {
                updateDialogue = true;
                if (npc_state.dialogueIndex > npc_d.dialogues.Length - 1)
                    CloseDialogue();

                dialogueText.text = npc_d.dialogues[npc_state.dialogueIndex].dialogText[lineIndex];
            }

            if (a_input)
            {
                lineIndex++;
                updateDialogue = false;

                if (lineIndex > npc_d.dialogues[npc_state.dialogueIndex].dialogText.Length - 1)
                {
                    if (npc_d.dialogues[npc_state.dialogueIndex].increaseIndex)
                    {
                        npc_state.dialogueIndex++;

                        if(npc_state.dialogueIndex > npc_d.dialogues.Length - 1)
                        {
                            npc_state.dialogueIndex = npc_d.dialogues.Length - 1;
                        }
                    }
                    CloseDialogue();
                }
            }
        }
        void CloseDialogue()
        {
            dialogueActive = false;
        }
        public static DialogueManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }

}
