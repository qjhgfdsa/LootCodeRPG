using UnityEngine;

namespace SA
{
    public class M_JoinRoom : MonoBehaviour
    {
        public RoomInfo roomInfo;

        public void Press()
        {
            Launchers.singleton.Button_JoinRoom(roomInfo);
        }
    }
}
