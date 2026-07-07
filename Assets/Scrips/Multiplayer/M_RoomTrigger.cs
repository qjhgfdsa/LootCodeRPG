using UnityEngine;


namespace SA
{
    public class M_RoomTrigger : MonoBehaviour
    {
        public string roomName = "room1";

        void OnTriggerEnter(Collider other)
        {
            StateManager states = other.transform.root.GetComponent<StateManager>();
            if (states != null)
                return;

            if (states.isLocal == false)
                return;

            if (states.isInRoom)
            {
                M_Launcher.singleton.LeaveRoom(states);
                
            }
            else
            {
                M_Launcher.singleton.JoinRoom(roomName, states);
            }
        }
    }
}